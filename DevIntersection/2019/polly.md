# Polly

AppvNext - current stewards of polly

Carl Franklin

## Transient Errors
- Network outages
- Service outages
and like 3 more I missed

works in blazor

_What's bulkhead isolation?_
_"one fault shouldn't sink the whole ship"_

fallback feature

## retry patterns
- retry (immediately; can set number of retries)
- wait and retry
  exponential back-off, jitter...
- retry forever

## Bulkhead isolation
1 fault shouldn't sink the who ship

## Cache feature
polly allows plugging in a cache ...

## Fallback
allows for failing gracefully

see photo for Policy Basics
![Resources](images/policy-basics.jpg)

Samples:github.com/app-vnext/polly-samples

see photo for retry code sample
![Resources](images/polly-code.jpg)
handles all exceptions by using the Exception base class

## Good combination
combine exponential backoff + (eventual) timeout + circuit breaker

"breaking the circuit" sounds more like "put requests on pause"
  "break the circuit" after N retries

## policy wrap
- goes from outer to inner

VS code on string wasn't versioned ... so different than what's on GH?

```
.Handle<Exception>
.OrInner<SomeInnerException>
```

## next release - chaos engineering 
called "Simmy" (like Simeon)

polly-js another option

see photo for resource links
![Resources](images/polly-wiki.jpg)
