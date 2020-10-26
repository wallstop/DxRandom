# Overview

DxRandom is a small set of Random utilities that have been developed over a period of years.

In addition to generating random primitive types, any type that derives from IRandom has the ability to find random elements of collections of types as well as enums.

Additionally, all random types are accessible in a thread-local fashion via the ThreadLocalRandom implementation, callable like so: ```ThreadLocalRandom<SystemRandom>.Instance.Next()```
