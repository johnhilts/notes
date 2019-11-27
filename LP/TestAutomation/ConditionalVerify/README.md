# ConditionalVerify

## ConditionalVerify: Skippable verify in case you provided a message.
### The point of ConditionalVerify is to skip a test if certain conditions are true.

# How `True` works:
## `try` block
* Do a bunch of logging with a message that potentially explains why a test would be skipped.
* Do an Xunit Assertion (true)
* If the assertion fails, catch the exception. 
## `catch` block
* If a server side error (500) occurred, then skip the test.
* More logging, this time including the Xunit exception's info.

```
        public override void True(bool expression, string message, bool skipMessage = false)
        {
            try
            {
                if (!skipMessage)
                {
                    LastVerify = VerifyType.True;
                    CheckMessageIsNotEmpty(message); // <-- will throw an exception if message is null or empty; note, it won't be caught in this method.
                    Browser.Log.AttemptToVerify(Type, VerifyType.True, ""); // <-- this doesn't actually verify anything, it just does a lot of logging 
                }
                Assert.True(expression); // <-- not sure why it's even necessary to use Xunit for this ... it's worse perf to use exceptions this way ...
            }
            catch (TrueException ex)
            {
                Browser.SkipTestIfServerError(); // <-- see code block below
                HandleTrueFailure(message, ex); // <-- almost the same thing as AttemptToVerify (just logging)
            }
        }

```

I'm suspicious as to whether this entire method even works as advertised ...
```
        public void SkipTestIfServerError()
        {
            if (IsInitialized) // <-- this is set at the end of the ctor for this object ... so it will always be true!
            {
                // Js makes sure body element exists, and if it does it returns the inner text.
                // Not using IWebElement for performance, and not using Browser.Locate.ElementImmediately to avoid recursion.
                var bodyText = ExecuteJs("return document.querySelector('body') && document.querySelector('body').innerText").ToString().ToLower();

                // If it can't find text of body, then we don't have the prerequisite to deem this a server error.
                if (string.IsNullOrWhiteSpace(bodyText))
                {
                    return;
                }

// not sure if MB was aware that .Contains does a case-sensitive comparison ...
                // Checks if on site by checking if specific string is contained in the source that wouldn't be present in a server error page.
                var isOnSite = PageSource.ToLower().Contains(_requiredStringInSource.ToLower()); // is the string "lamps plus" present?
                var matchErrorCode = new Regex($@"({HttpStatusCode.NotFound}|{HttpStatusCode.InternalServerError}|{HttpStatusCode.ServiceUnavailable})").Match(bodyText);
                var isServerErrorPage = bodyText.Contains("error") && matchErrorCode.Success && !isOnSite; // string match on entire page ... 

		// I don't think this was ever true ... checked an older log, and didn't find the "Server Error " string
                if (isServerErrorPage)
                {
                    if (!string.IsNullOrEmpty(matchErrorCode.Value))
                    {
                        Skip.If(true, $"Server Error {matchErrorCode.Value}: {System.Web.HttpWorkerRequest.GetStatusDescription(Convert.ToInt32(matchErrorCode.Value))}");
                    }
                }
            }
            else
            {
                Log.Message("WARNING: The Browser was not initialized");
            }
        }

```

