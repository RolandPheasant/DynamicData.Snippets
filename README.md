# Dynamic Data Snippets

People are always asking me for documents or explanations of dynamic data. However as an open source developer and active family man who works in a high pressure environment, the maintenance and development of my open source projects leaves me with no time to even consider documents.  

Thinking about it, why do people always ask for documents. We are developers and we can write and analyse code better than we can produce documents (well most of us anyway).  That is why I have created this project, with the aim to:

 - Regard this project as a '101 Samples project'
 - Ensure all examples are unit tested
 - Respond to queries from the community by adding new examples to the project
 
### Support And Contact

The dynamic data chat room is a sub channel the Reactive Inc slack channel. It is an invite only forum so if you want an invite send me a message @ roland_pheasant@hotmail.com. If you would like me to produce an example to help with a particular problem, feel free to contact me on slack to discuss it further.

### Links to examples
All these examples have working unit tests which allows for debugging and experimentation

| Topic| Link| Description|
| ------------- |-------------| -----|
| Aggregation      |[Aggregations.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Aggregation/Aggregations.cs) | Dynamically aggregrate items in a data source |
| Creation      |[ChangeSetCreation.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Creation/ChangeSetCreation.cs) | Create list and cache using first class observables |
| Filtering      |[StaticFilter.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Filter/StaticFilter.cs) | Filter a data source using a static predicate |
| |[DynamicFilter.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Filter/DynamicFilter.cs) | Create and apply an observable predicate |
| |[ExternalSourceFilter.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Filter/ExternalSourceFilter.cs) | Create an observable predicate from another data source |
| |[PropertyFilter.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Filter/PropertyFilter.cs) | Filter on a property which implements ```INotifyPropertyChanged```  |
| Grouping|[CustomTotalRows.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Group/CustomTotalRows.cs) | Illustrate how grouping can be used for custom aggregation  |
| |[XamarinFormsGrouping.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Group/XamarinFormsGrouping.cs) | Bespoke grouping with Xamarin Forms  |
| |[GroupAndMonitorPropertyChanges.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Group/GroupAndMonitorPropertyChanges.cs) | Group on the first letter of a property and update grouping when the property changes |
| Inspect Collection|[InspectCollection.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/InspectItems/InspectCollection.cs) | Produce an observable based on the contents of the datasource  |
| |[InspectCollectionWithPropertyChanges.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/InspectItems/InspectCollectionWithPropertyChanges.cs) | Produce an observable based on the contents of the data source, which also fires when a specified property changes |
| |[InspectCollectionWithObservable.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/InspectItems/InspectCollectionWithObservable.cs) | Produce an observable based on the contents of the data source, whose values are supplied by an observable on each item in the collection|
| |[MonitorSelectedItems.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/InspectItems/MonitorSelectedItems.cs) | Monitor a collection of items which have an IsSelected property and produce observables based on selection|
| Mutable Values|[AutoRefreshForPropertyChanges.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/MutableValues/AutoRefreshForPropertyChanges.cs) | How to force cache operators to recalulate when using mutable objects|
| Sorting|[ChangeComparer.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Sorting/ChangeComparer.cs) | How to dynamically sort a collection using an observable comparer|
| |[CustomBinding.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Sorting/CustomBinding.cs) | Customise binding behaviour for a sorted data source|
| Switch |[SwitchDataSource.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Switch/SwitchDataSource.cs) | Toggle between different data sources|
| Transform |[FlattenNestedObservableCollection.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Transform/FlattenNestedObservableCollection.cs) | Flatten nested observable collections into an observable data source |
| Testing|[ViewModel.cs](https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/ViewModelTesting/ViewModel.cs) | Illustrates how to test a view model when using dynamic data|

### Real world examples of Dynamic Data

I have created several Dynamic Data in action projects which illustrates the usage of dynamic data. I encourage people who want to see these real world examples to take a look at the following projects to see the capabilities of Dynamic Data.

Include are:

[Dynamic Trader](https://github.com/RolandPheasant/Dynamic.Trader) which is an example of how Dynamic Data can handle fast moving high throughput trading data with. It illustrates some of the core operators of dynamic data and how a single data source can be shared and shaped in various ways. It also includes an example of how it can be integrated with ReactiveUI.

![Dynamic Trader](https://github.com/RolandPheasant/TradingDemo/blob/master/Images/LiveTrades.gif)

[TailBlazer](https://github.com/RolandPheasant/TailBlazer) is a popular file tail utility which is an example of Rx and Dynamic Data and I consider to be a celebration of reactive programming. It is an advanced example of how to achieve high performance and how to lean on Rx and Dynamic Data to produce a slick and response user interface.

![Tail Blazer](https://github.com/RolandPheasant/TailBlazer/blob/master/Images/Release%20v0.9/Search%20and%20highlight.gif)

[DynamicData Samplz](https://github.com/RolandPheasant/DynamicData.Samplz) where I started to do some visual examples but abandoned the project because I decided that the snippets project would be a better means of providing quick to produce examples. However there are still several good examples so well worth taking a look.

![Samplz](https://github.com/RolandPheasant/DynamicData.Samplz/blob/master/Images/Screenshot.gif)


