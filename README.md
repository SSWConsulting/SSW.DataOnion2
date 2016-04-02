# SSW.DataOnion2

SSW Data Onion helps when implementing a persistence ignorant Repository pattern (often used with Onion Architecture) with Entity Framework. It also allows you to create and manage lifecycle of the Ambient DbContext which can be used across mutliple threads.

There are two implementations of DataOnion2:

1. Entity Framework 6 compatible implementation built on .NET 4.5 framework
2. Entity Framework Core 1 (aka 7) compatible implementation that can be used in dnx451 and dnxcore5 projects.

Use [EF6](https://github.com/SSWConsulting/SSW.DataOnion2/tree/master/EF6) and [EF7](https://github.com/SSWConsulting/SSW.DataOnion2/tree/master/EF7) folders to view details of each implementation.
