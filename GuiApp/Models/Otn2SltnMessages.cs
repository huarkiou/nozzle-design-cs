using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GuiApp.Models;

public class BaseFieldValueChangedMessages(string value) : ValueChangedMessage<string>(value);

public class NozzleSizeValueChangedMessages(double hInlet, double hOutlet)
    : ValueChangedMessage<(double, double)>((hInlet, hOutlet));