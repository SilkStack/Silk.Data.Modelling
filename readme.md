# Silk.DataModelling

## Overview

Silk.DataModelling is a library for modelling data structures for .NET.

Using Silk.DataModelling will allow you to use conventions to translate data between different problem domains such as ORM mapping, wire formats or cache servers.

### Platforms

Silk.DataModelling supports netstandard2.0.

### Mapping

Silk.DataModelling provides an extensible API for mapping data between domains, that is, from models to views and back again.

Library authors are able to provide their own mapping conventions, data sources and even translate data as it's mapped - with full async support!

### Models

Models are a representation of a data structure, commonly representing a CLR `Type`.

A model looks a lot like .NETs reflection but is abstracted away to work with more than just object instances.

### Views

A view is a translation of a model for a specific problem domain. For example, a business object might have a view that represents the persistent storage schema in a database.

Views are built from a model and a set of rules or conventions that decide how the view should be assembled.

### Containers

Containers are objects that contain your data, instances of your model or view.

These can be .NET CLR types, database record rows, JSON objects or anything else suitable for storing data.

## Custom View Modelling

## Transformations

## License

Silk.DataModelling is licensed under the MIT license.