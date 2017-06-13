# SimpleMapper

[![rolosoft_public_packages MyGet Build Status](https://www.myget.org/BuildSource/Badge/rolosoft_public_packages?identifier=a3bd8d82-142b-45aa-af8f-da7486b6a371)](https://www.myget.org/)

## About
SimpleMapper is built from the ground up with performance and modern testability software practices in mind.
It is a light-weight tool for mapping between objects across software tiers.
Efficient data tier mapping is achieved using LINQ projection whilst in-memory mappings can be defined by implementing the IMapper<TSource, TDestination> interface.

## Features
 * _Speed_ - very few reflection based operations. Where reflection is used, expression trees are cached in a thread safe, strategic way using [MemoryCache](http://msdn.microsoft.com/en-us/library/vstudio/system.runtime.caching.memorycache).
 * _Efficiency_ - just returns the data needed from the data tier rather than "greedy" in-memory hosepipe fetches.
 * _Declarative mapping_ - Define maps declaratively at design time by implementing IMapper<TSource, TDestination> and then register these maps with your preferred IoC container (e.g. NinJect, Windsor, Unity, SimpleInjector etc).
 * _Testability_ - Mock and stub maps using popular mock frameworks.
 * _Thread safe_ - Designed with parallel processing thread safety in mind.
 * _Map lifetime control_ - Control map lifetime (e.g. singleton, per call) by leveraging advanced lifetime scoping control capabilities of most IoC containers.

## Installation
Via [nuget](http://www.nuget.org/packages/SimpleMapper/)

~~~
Install-Package SimpleMapper
~~~

## FAQ

Q. _Why should I use this and not AutoMapper?_

A. We love AutoMapper and have used it a lot over the years! It has, however, become a bit of a "swiss army knife" in recent times focusing on doing a lot across many platforms such as Silverlight and Win 8. 
SimpleMapper provides efficient and fast mapping capabilities without the bloat of cross-platform or reflection based automated wiring capabilities.

Q. _What type of mapping is supported?_

A. Two types of mapping are supported:

 1. _Data to business tier_ IQueryable<T> automated mapping using [LINQ projection](https://www.google.com/?q=linq%20projection).
 2. _In memory_ declarative mapping useful for Business to Service Tier.


Q. _How do I create "in memory" maps?_

A. We suggest a three step approach:

 1. Create maps by implementing IMapper<TSource, TDestination>.
 2. Register map implementations in an IoC container.
 3. Inject mapping modules into classes using Constructor Injection techniques.

Q. _Can it do multi-level mapping?_ 

A. It depends. 

*In memory maps*

You can do anything from user defined maps implementing IMapper<TSource, TDestination>!

*Projection*

Projection extensions support 1st level only mapping. 
This is by design for performance reasons. When fetching data from persistence stores, best performance is achieved by fetching from a single object (e.g. table, view or single result set stored procedure).
Please use the in memory map IMapper<TSource, TDestination> to merge flat projection entities into rich, hierarchical memory object graphs.

 
## Example usage

### Scenario 1 - Data Access Mapping from Entity Framework
Using LINQ projection to access data has many performance advantages such as:

 * eliminating the possibility for multiple database calls that happen due to lazy loading from many ORMs.
 * honour the architectural principal of moving "minimum data" across tiers (especially relevant at the database <-> business tier)

```C#

public sealed class MyBizObject
{
	public int Id{get;set;}
	public string Name{get;set;}
}

public sealed class MyRepository
{
	public IList<MyBizObject> GetAll()
	{
		var rtn = new List<MyBizObject>();
		using(var context = new MyContext())
			{
				rtn = context.Persons.Project().To<MyBizObject>();
			}
		}
		return rtn;
}
```
	
The above calls result in efficient SQL similar to:

```
#!sql
SELECT [Id],[Name]
FROM dbo.Person
```

Note:

 * Only the columns requested are fetched (rather than SELECT *.....)
 * No related entities are fetched (e.g. joins)
 * Only one database operation occurs. 
 
The above notes would _not_ be the case when using some in-memory automated mappers.


### Scenario 2 - In Memory Mapping (e.g. Data Transfer Objects (DTOs)

#### Step 1 - Defining the Map

##### The DTOs

```C#
public class MyBizObject
{
	public int Id{get;set;}
	public string Name{get;set;}
}
public class MyDTOObject
{
	public int Id{get;set;}
	public string FullName{get;set;}
}
```

##### The Map

```C#
public sealed class MyDTOMap : IMapper<MyBizObject, MyDTOObject>
{
	public MyDTOObject Map(MyBizObject source)
	{
		var rtn = new MyDTOObject(){Id = source.Id, FullName = source.Name};
		
		return rtn;
	}
}
```

#### Step 2 - Registering the Map

```C#
var container = new Container(); // e.g. SimpleInjector
container.Register<IMapper<MyBizObject, MyDTOObject>, MyDTOMap>();
```

#### Step 3 - Use the Map

```C#
public class MyClassNeedingAMap
{
	private readonly IMapper<MyBizObject, MyDTOObject> mapper;

	// constructor injection
	public MyClassNeedingAMap(IMapper<MyBizObject, MyDTOObject> mapper)
	{
		this.mapper = mapper;
	}

	public void MethodNeedingMapping()
	{
		// create biz object
		var bizObj = new MyBizObject{Id=1, Name="Bob Blog"};
		
		// do the map
		var DTOObj = this.mapper.Map(bizObj);
		
		// process the mapped object
		DoSomeProcess(DTOObj);
		
	}
	
}
```

### Miscellaneous 

#### Lifetime Management of Maps.
By leveraging the lifetime management models of IoC controllers, it is possible to to control the instancing of maps.
For example, for an app domain level singleton map instance managed with SimpleInjector:

```C#
// 1. Create a new Simple Injector container
var container = new Container();

// 2. Configure the container (register with singleton lifetime scoping)
container.Register<IMapper<MyBizObject, MyDTOObject>, MyDTOMap>(Lifestyle.Singleton);
```
