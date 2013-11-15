using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using NSB.Libs;

namespace NSB
{
    public static class NSBridge
	{
	


        static bool protocolRegistered = false;

        public static void EnableNSBridge() {
            if (!protocolRegistered) {              
                NSUrlProtocol.RegisterClass (new MonoTouch.ObjCRuntime.Class (typeof (AppProtocolHandler)));

                protocolRegistered = true;
            }
        }

        public static void InjectMtJavascript(this UIWebView webView) {
            webView.EvaluateJavascript(NSBRIDGE_JAVASCRIPT);
        }
		
		private static List<EventListener> EventListeners = new List<EventListener>();
		
		public static void AddEventListener (this UIWebView source, string EventName, Action<FireEventData> Event) {
			EventListeners.Add( new EventListener(source, EventName, Event) );
		}
		
		public static void RemoveEventListener (this UIWebView source, string EventName, Action<FireEventData> Event) {
			for(int xx = 0; xx < EventListeners.Count; xx++) {
				var ee = EventListeners[xx];
				if (source == ee.WebView 
				    && string.Compare(EventName, ee.EventName, StringComparison.InvariantCultureIgnoreCase) == 0
					&& ee.Event == Event) {
					EventListeners.RemoveAt(xx);
					break;
				}
			}
		}
		
		public static void FireEvent (this UIWebView source, string EventName, Object Data) {
			// call javascript event hanlder code

			string json = SimpleJson.SerializeObject(Data);
            source.BeginInvokeOnMainThread ( delegate{ 
                source.EvaluateJavascript(string.Format("NSBridge.App._dispatchEvent('{0}', {1});", EventName, json));
            });
		}
		
		public static void JsEventFired (FireEventData feData) {
			foreach(var ee in EventListeners.Where(oo => string.Compare(oo.EventName, feData.Name, StringComparison.InvariantCultureIgnoreCase) == 0)) {				
				ee.Event(feData);				
			}
		}

        #region NSBridge_JAVASCRIPT
        private static string NSBRIDGE_JAVASCRIPT = @"
(function (global) {
    var SETTIMEOUT = global.setTimeout, 
        doc = global.document,
        callback_counter = 0;

    global.jXHR = function () {
        var script_url,
            script_loaded,
            jsonp_callback,
            scriptElem,
            publicAPI = null;

        function removeScript() { try { scriptElem.parentNode.removeChild(scriptElem); } catch (err) { } }

        function reset() {
            script_loaded = false;
            script_url = '';
            removeScript();
            scriptElem = null;
            fireReadyStateChange(0);
        }

        function ThrowError(msg) {
            try { publicAPI.onerror.call(publicAPI, msg, script_url); } catch (err) { throw new Error(msg); }
        }

        function handleScriptLoad() {
            if ((this.readyState && this.readyState !== 'complete' && this.readyState !== 'loaded') || script_loaded) { return; }
            this.onload = this.onreadystatechange = null; 
            script_loaded = true;
            if (publicAPI.readyState !== 4) ThrowError('Script failed to load [' + script_url + '].');
            removeScript();
        }

        function fireReadyStateChange(rs, args) {
            args = args || [];
            publicAPI.readyState = rs;
            if (typeof publicAPI.onreadystatechange === 'function') publicAPI.onreadystatechange.apply(publicAPI, args);
        }

        publicAPI = {
            onerror: null,
            onreadystatechange: null,
            readyState: 0,
            open: function (method, url) {
                reset();
                internal_callback = 'cb' + (callback_counter++);
                (function (icb) {
                    global.jXHR[icb] = function () {
                        try { fireReadyStateChange.call(publicAPI, 4, arguments); }
                        catch (err) {
                            publicAPI.readyState = -1;
                            ThrowError('Script failed to run [' + script_url + '].');
                        }
                        global.jXHR[icb] = null;
                    };
                })(internal_callback);
                script_url = url.replace(/=\?/, '=jXHR.' + internal_callback);
                fireReadyStateChange(1);
            },
            send: function () {
                SETTIMEOUT(function () {
                    scriptElem = doc.createElement('script');
                    scriptElem.setAttribute('type', 'text/javascript');
                    scriptElem.onload = scriptElem.onreadystatechange = function () { handleScriptLoad.call(scriptElem); };
                    scriptElem.setAttribute('src', script_url);
                    doc.getElementsByTagName('head')[0].appendChild(scriptElem);
                }, 0);
                fireReadyStateChange(2);
            },
            setRequestHeader: function () { }, // noop
            getResponseHeader: function () { return ''; }, // basically noop
            getAllResponseHeaders: function () { return []; } // ditto
        };

        reset();

        return publicAPI;
    };
})(window);

NSBridge = {};
NSBridge.appId = 'nsbridge';
NSBridge.pageToken = 'index';
NSBridge.App = {};
NSBridge.API = {};
NSBridge.App._listeners = {};
NSBridge.App._listener_id = 1;
NSBridge.App.id = NSBridge.appId;
NSBridge.App._xhr = jXHR;
NSBridge._broker = function (module, method, data) {
    var x1 = new NSBridge.App._xhr();
    x1.onerror = function (e) {
        console.log('XHR error:' + JSON.stringify(e));
    };
    var url = 'app://' + module + '/' + method + '?callback=?&data=' + encodeURIComponent(JSON.stringify(data)) + '&_=' + Math.random();
    //Console.log(url);
    x1.open('GET', url);
    x1.send();
};
NSBridge._hexish = function (a) {
    var r = '';
    var e = a.length;
    var c = 0;
    var h;
    while (c < e) {
        h = a.charCodeAt(c++).toString(16);
        r += '\\\\u';
        var l = 4 - h.length;
        while (l-- > 0) {
            r += '0'
        }
        ;
        r += h
    }
    return r
};
NSBridge._bridgeEnc = function (o) {
    return'<' + NSBridge._hexish(o) + '>'
};
NSBridge.App._JSON = function (object, bridge) {
    var type = typeof object;
    switch (type) {
        case'undefined':
        case'function':
        case'unknown':
            return undefined;
        case'number':
        case'boolean':
            return object;
        case'string':
            if (bridge === 1)return NSBridge._bridgeEnc(object);
            return '""""' + object.replace(/""""/g, '\\\\""""').replace(/\\n/g, '\\\\n').replace(/\\r/g, '\\\\r') + '""""'
    }
    if ((object === null) || (object.nodeType == 1))return'null';
    if (object.constructor.toString().indexOf('Date') != -1) {
        return'new Date(' + object.getTime() + ')'
    }
    if (object.constructor.toString().indexOf('Array') != -1) {
        var res = '[';
        var pre = '';
        var len = object.length;
        for (var i = 0; i < len; i++) {
            var value = object[i];
            if (value !== undefined)value = NSBridge.App._JSON(value, bridge);
            if (value !== undefined) {
                res += pre + value;
                pre = ', '
            }
        }
        return res + ']'
    }
    var objects = [];
    for (var prop in object) {
        var value = object[prop];
        if (value !== undefined) {
            value = NSBridge.App._JSON(value, bridge)
        }
        if (value !== undefined) {
            objects.push(NSBridge.App._JSON(prop, bridge) + ': ' + value)
        }
    }
    return'{' + objects.join(',') + '}'
};


NSBridge.App._dispatchEvent = function (type, evt) {
    var listeners = NSBridge.App._listeners[type];
    if (listeners) {
        for (var c = 0; c < listeners.length; c++) {
            var entry = listeners[c];
                entry.callback.call(entry.callback, evt)
        }
    }
};
NSBridge.App.fireEvent = function (name, evt) {
    NSBridge._broker('App', 'fireEvent', {name:name, event:evt})
};
NSBridge.API.log = function (a, b) {
    NSBridge._broker('API', 'log', {level:a, message:b})
};
NSBridge.API.debug = function (e) {
    NSBridge._broker('API', 'log', {level:'debug', message:e})
};
NSBridge.API.error = function (e) {
    NSBridge._broker('API', 'log', {level:'error', message:e})
};
NSBridge.API.info = function (e) {
    NSBridge._broker('API', 'log', {level:'info', message:e})
};
NSBridge.API.fatal = function (e) {
    NSBridge._broker('API', 'log', {level:'fatal', message:e})
};
NSBridge.API.warn = function (e) {
    NSBridge._broker('API', 'log', {level:'warn', message:e})
};
NSBridge.App.addEventListener = function (name, fn) {
    var listeners = NSBridge.App._listeners[name];
    if (typeof(listeners) == 'undefined') {
        listeners = [];
        NSBridge.App._listeners[name] = listeners
    }
    var newid = NSBridge.pageToken + NSBridge.App._listener_id++;
    listeners.push({callback:fn, id:newid});
};
NSBridge.App.removeEventListener = function (name, fn) {
    var listeners = NSBridge.App._listeners[name];
    if (listeners) {
        for (var c = 0; c < listeners.length; c++) {
            var entry = listeners[c];
            if (entry.callback == fn) {
                listeners.splice(c, 1);
                break
            }
        }
    }
};";
        #endregion
		
	}
	

    public class AppProtocolHandler : NSUrlProtocol {

        [Export ("canInitWithRequest:")]
        public static bool canInitWithRequest (NSUrlRequest request)
        {
            // from the NSBridgeJS to the Native Handler 
            // var url = 'app://' + module + '/' + method + '?callback=?&data=' + encodeURIComponent(JSON.stringify(data)) + '&_=' + Math.random();
            return request.Url.Scheme == "app";
        }

        [Export ("canonicalRequestForRequest:")]
        public static new NSUrlRequest GetCanonicalRequest (NSUrlRequest forRequest)
        {
            return forRequest;
        }

        [Export ("initWithRequest:cachedResponse:client:")]
        public AppProtocolHandler (NSUrlRequest request, NSCachedUrlResponse cachedResponse, NSUrlProtocolClient client) 
            : base (request, cachedResponse, client)
        {
        }

        public override void StartLoading ()
        {
			var parameters = Request.Url.Query.Split('&');
			if (parameters.Length > 2) {
				var callbackToks = parameters[0].Split('=');
				var dataToks = parameters[1].Split('=');
				if (callbackToks.Length > 1 && dataToks.Length > 1) {

					var appUrl = new AppUrl() {
						Module = Request.Url.Host,
						Method = Request.Url.RelativePath.Substring(1),
                        // TODO: Move away from this approach of JSON handling
						JsonData = System.Web.HttpUtility.UrlDecode(dataToks[1])
					};

					// this is a request from mt.js so handle it.
					switch (appUrl.Module.ToLower()) 
					{
						case "app":
    						if (string.Equals(appUrl.Method, "fireEvent", StringComparison.InvariantCultureIgnoreCase)) {
    							// fire this event.
    							var feData = appUrl.DeserializeFireEvent();
    							// find event listeners for this event and trigger it.
                                NSBridge.JsEventFired(feData);
    						}

						    break;

						case "api":
    						if (string.Equals(appUrl.Method, "log", StringComparison.InvariantCultureIgnoreCase)) {
    							
    							var lData = appUrl.DeserializeLog();
                                Console.WriteLine("BROWSER:[" + lData.Level + "]: " + lData.Message);

    						}

						    break;
					}

					// indicate success.
					var data = NSData.FromString(callbackToks[1] + "({'success' : '1'});");
                    // TODO: Need to create a pool of static response objects for faster servicing
					using (var response = new NSUrlResponse (Request.Url, "text/javascript", Convert.ToInt32(data.Length), "utf-8")) {
						Client.ReceivedResponse (this, response, NSUrlCacheStoragePolicy.NotAllowed);
						Client.DataLoaded (this, data);
						Client.FinishedLoading (this);
					}

					return;                            

				}
			}

			Client.FailedWithError(this, NSError.FromDomain(new NSString("AppProtocolHandler"), Convert.ToInt32(NSUrlError.ResourceUnavailable)));
			Client.FinishedLoading(this);
        }

        public override void StopLoading ()
        {
        }
        
    }


	public class EventListener {
		public UIWebView WebView { get; set; }
		public string EventName { get; set; }
		public Action<FireEventData> Event { get; set; }
		
		public EventListener() {
		}
		
		public EventListener(UIWebView WebView, string EventName, Action<FireEventData> Event) {
			this.WebView = WebView;
			this.EventName = EventName;
			this.Event = Event;
		}
	}
	
	public class AppUrl {
		public string Module { get; set; }
		public string Method { get; set; }
		public string JsonData { get; set; }
		
		public Object Deserialize() {
            // TODO: Move away from SimpleJson with MT core JSON handlers
			return SimpleJson.DeserializeObject(JsonData);
		}
		
		public T Deserialize<T>() {
            // TODO: Move away from SimpleJson with MT core JSON handlers
			return SimpleJson.DeserializeObject<T>(JsonData);
		}
		
		public FireEventData DeserializeFireEvent() {
			if (string.Equals(Method, "fireEvent", StringComparison.InvariantCultureIgnoreCase)) {
				return new FireEventData(JsonData);
			}
			else {
				return null;
			}
		}
		
		public LogData DeserializeLog() {
			if (string.Equals(Method, "log", StringComparison.InvariantCultureIgnoreCase)) {
				return new LogData(JsonData);
			}
			else {
				return null;
			}
		}				
	}
	

	public class FireEventData {
		public string Name { get; set; }
		public JsonObject Data { get; set; }
		public string JsonData { get; set; }
		
		public FireEventData() {
		}
		
		public FireEventData(string Json) {
			JsonObject feData = (JsonObject)SimpleJson.DeserializeObject(Json);
			this.Name = feData["name"].ToString();
			this.Data = (JsonObject)feData["event"];
			// save json of data so user can desiralize in a typed object.
			this.JsonData = SimpleJson.SerializeObject(this.Data);
		}
	}
	

	public class LogData {
		public string Level { get; set; }
		public string Message { get; set; }
		
		public LogData() {
		}
		
		public LogData(string Json) {
			JsonObject lData = (JsonObject)SimpleJson.DeserializeObject(Json);
			this.Level = lData["level"].ToString();
            if (lData["message"] != null) {
			    this.Message = lData["message"].ToString();
            }
		}
	}



}

