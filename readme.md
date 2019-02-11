# Silk.Data.Modelling

`Silk.Data.Modelling` is a .NET library for working with data structures and meta data.

## Installing

`Silk.Data.Modelling` is available as a NuGet package: https://www.nuget.org/packages/Silk.Data.Modelling

You can install it from the NuGet package manager in Visual Studio or from command line with dotnet core:

~~~~
dotnet add package Silk.Data.Modelling
~~~~

## Platform Requirements

`Silk.Data.Modelling` is built for netstandard2.0.

## License

`Silk.Data.Modelling` is licensed under the MIT license.

## Usage

- Map objects with `ObjectMapper`.
- Examine the model of a .NET CLR type with `TypeModel`.
- Examine how two models intersect with `TypeToTypeIntersectionAnalyzer`.
- Generate a mapping with `TypeToTypeMappingFactory`.
- Read and write to an object graph with `ObjectGraphReaderWriter`.

### Map objects with `ObjectMapper`

`ObjectMapper` is the standard implementation for mapping data from one type to another.

Use is pretty straightforward:

~~~~
var mapper = new ObjectMapper();
var typeBObj = mapper.Map<TypeA, TypeB>(typeAObj);
~~~~

The `ObjectMapper` will accept custom configurations for analyzing, mapping building, type instance factory and graph reader/writer factory, allowing the developer greater control of how mappings work in different scenarios.

### Examine the model of a .NET CLR type with `TypeModel`

`TypeModel` is the structure used to examine metadata of a CLR type. To retrieve the `TypeModel` of a type simply:

~~~~
TypeModel<TypeA> typeAModel = TypeModel.GetModelOf<TypeA>();
~~~~

Once you have the `TypeModel` you can examine it's metadata or you can pass it into more interesting APIs like...

### Examine how two models intersect with `TypeToTypeIntersectionAnalyzer`

`TypeToTypeIntersectionAnalyzer` is the type used to analyze two models and determine what they have in common.

An intersection analysis consists of two main stages (and behavior of both is completely customizable):

- Analyze the two models and find candidate fields that could possibly intersect
    - This is goverened by `IIntersectCandidateSource` implementations the developer can provide
- Filter the candidate fields against a set of rules of what's interesting
    - These are `IIntersectionRule` implementations the developer can provide
    
Once you have the intersection you could use it directly (for, say, determining how an entity type relates to it's database storage schema) or you can...

### Generate a mapping with `TypeToTypeMappingFactory`

`TypeToTypeMappingFactory` uses similar rules to `TypeToTypeIntersectionAnalyzer` to generate a mapping between two types. Like other APIs in `Silk.Data.Modelling` you can provide a custom set of binding rules (`IBindingFactory` implementations) to govern how models are bound.

### Read and write to an object graph with `ObjectGraphReaderWriter`

`ObjectGraphReaderWriter` is an abstraction API for reading and writing values to a graph of type instances.

## Extending

`Silk.Data.Modelling` is built so that all of the above can be extended. Developers can develop their own data models, analyzers, mappings and graph read/write APIs that will work with the analysis rules that come with the library out of the box.

### Custom Models

Model and field symbols are strongly typed throughout the `Silk.Data.Modelling` APIs while, largely, being completely agnostic to the type of model or field being used.

A custom model and field structure can be designed by simply implementing `IModel` and `IField` and populating objects with the data your model needs.

For an example of this you can look at [Silk.Data.SQL.ORM](https://github.com/SilkStack/Silk.Data.SQL.ORM) which comes with it's own custom model for modelling how entities are stored in a database and mapping between entity types and the database.

### Graph Readers + Writers

The graph reading and writing APIs implement how to store and retrieve information in your data graph. The standard implementation is `ObjectGraphReaderWriter` which will store data in instances of CLR types. However, none of the standard bindings actually create instance factories or the like, instead focusing purely on the mapping and storage of values - the graph reader/writer API decides how to create instances of the require types.

To implement your own graph reader and writer you need to implement `IGraphReader` and `IGraphWriter`.