# Silk.Data.Modelling

## Overview

A library for .NET for handling data modelling, model transformations and mapping tasks.

### APIs

* *Modelling* - Represent data structures as manipulable meta data. 
* *Model transformations* - Transforms data models into problem-domain specific models.
* *Mapping* - Map data between models.

### Platforms

Silk.Data.Modelling supports netstandard1.3.

## Modelling

Modelling converts and represents your data structures into metadata that can be used for many different purposes - this library uses it for mapping. Models are implementations of `IModel<TField>` that can be customized to expose metadata needed by your applications.

### TypeModel

The built in model is `TypeModel` and it's type-specific implementation `TypeModel<T>`. A `TypeModel` instance represents a .NET `Type`.

	var modelOfObject = TypeModel.GetModelOf<object>();

## Model Transformations

Transformations allow you to change an existing model into a model more suitable for your applications purposes. Say you want to derive a model for persisting a cached version of a .NET object you could transform a `TypeModel` instance into your own `CacheModel` model and expose any extra metadata you need.

To accomplish this implement `IModelTransformer` and call `IModel.Transform`.

## Mapping

Mapping uses metadata from models to transform, copy or map data from one model instance to another. It's important to realize that mapping isn't limited to reading and writing to objects, through implementing an `IModelReadWriter` you could write to any data structure of your choosing.

### Object Mapping

The simplest operation is mapping between different objects. Doing this just needs an instance of `ObjectMapper`.

	var objectMapper = new ObjectMapper();
	var toObject = objectMapper.Map<ToType>(fromObject);

### Options

The `ObjectMapper` constructor can take 2 arguments:

* *MappingOptions* - Specifies conventions to be used when mapping, defaults to a common sense collection of conventions for most use cases.
* *MappingStore* - Specifies a cache of mappings, carrying this between `ObjectMapper` instances will prevent mappings being generated multiple times.

### Conventions

Conventions define the rules that mappings follow. When a mapping is generated each convention is executed in turn to create any supported bindings between models.

Developers can author their own conventions and their own bindings to suit the needs of their applications.

### Custom Mapping

If you want to map to something other than objects, or use your own `IModel` implementations use `MappingBuilder`.

	var mappingOptions = new MappingOptions();
	mappingOptions.Conventions.Add(new MyConvention());
	var mappingBuilder = new MappingBuilder(fromModel, toModel, mappingOptions);
	var mapping = mappingBuilder.BuildMapping();
	mapping.PerformMapping(fromReadWriter, toReadWriter);

## License

Silk.Data.Modelling is licensed under the MIT license.