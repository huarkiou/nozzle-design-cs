using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GuiApp.Models;

public class BaseFluidFieldMessage(string value) : ValueChangedMessage<string>(value);