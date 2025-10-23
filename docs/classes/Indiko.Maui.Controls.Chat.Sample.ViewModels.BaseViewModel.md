# Class: BaseViewModel

| Property | Value |
|----------|-------|
| Namespace | `Indiko.Maui.Controls.Chat.Sample.ViewModels` |
| Type | Class |
| Source File | `samples/Indiko.Maui.Controls.Chat.Sample/ViewModels/BaseViewModel.cs` |
| Modifiers | public, abstract, partial |
| Generated | 2025-10-23 05:48:17 UTC |

## Signature

```csharp
public abstract partial class BaseViewModel : ObservableObject, IViewModel
```

## Relationships

**Inherits from:** [ObservableObject](ObservableObject.md)

**Implements:**
- [IViewModel](IViewModel.md)

**Dependencies:**
- [bool](bool.md)
- [IViewModel](IViewModel.md)
- [object](object.md)
- [ObservableObject](ObservableObject.md)
- [Task](Task.md)

## Fields

### isBusy

```csharp
private bool isBusy
```

**Returns:** `bool`

## Methods

### OnAppearing

```csharp
public abstract Task OnAppearing(object param)
```

**Parameters:**

| Name | Type | Description | Default |
|------|------|-------------|---------|
| `param` | `object` |  | `` |

**Returns:** `Task`

