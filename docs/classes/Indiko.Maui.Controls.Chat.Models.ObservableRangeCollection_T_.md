# Class: ObservableRangeCollection

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Models` |
| Type | Class |
| Source File | `src/Indiko.Maui.Controls.Chat/Models/ObservableRangeCollection.cs` |
| Modifiers | public |
| Generated | 2025-10-23 05:41:50 UTC |

## Signature

```csharp
public class ObservableRangeCollection<T> : ObservableCollection<T>
```

## Relationships

**Inherits from:** [ObservableCollection<T>](ObservableCollection_T_.md)

**Dependencies:**
- [bool](bool.md)
- [IEnumerable<>](IEnumerable__.md)
- [int](int.md)
- [List<>](List__.md)
- [NotifyCollectionChangedAction](NotifyCollectionChangedAction.md)
- [ObservableCollection<T>](ObservableCollection_T_.md)
- [void](void.md)

## Constructors

### ObservableRangeCollection

```csharp
public .ctor()
```

### ObservableRangeCollection

```csharp
public .ctor(IEnumerable<T> collection)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `collection` | `IEnumerable<T>` |  | `` |

## Methods

### AddRange

```csharp
public void AddRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = null)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `collection` | `IEnumerable<T>` |  | `` |
| `notificationMode` | `NotifyCollectionChangedAction` |  | `` |

### RemoveRange

```csharp
public void RemoveRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = null)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `collection` | `IEnumerable<T>` |  | `` |
| `notificationMode` | `NotifyCollectionChangedAction` |  | `` |

### Replace

```csharp
public void Replace(T item)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `item` | `T` |  | `` |

### ReplaceRange

```csharp
public void ReplaceRange(IEnumerable<T> collection)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `collection` | `IEnumerable<T>` |  | `` |

### AddArrangeCore

```csharp
private bool AddArrangeCore(IEnumerable<T> collection)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `collection` | `IEnumerable<T>` |  | `` |

**Returns:** `bool`

### RaiseChangeNotificationEvents

```csharp
private void RaiseChangeNotificationEvents(NotifyCollectionChangedAction action, List<T>? changedItems = null, int startingIndex = -1)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `action` | `NotifyCollectionChangedAction` |  | `` |
| `changedItems` | `List<T>?` |  | `` |
| `startingIndex` | `int` |  | `-1` |

