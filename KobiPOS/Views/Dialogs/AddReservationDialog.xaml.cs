// KİŞİ SAYISI PARSE - GÜVENLİ YOL
int guestCount = 4; // Default
var selectedGuestItem = GuestCountComboBox.SelectedItem as ComboBoxItem;
if (selectedGuestItem != null && selectedGuestItem.Content != null)
{
    string contentValue = selectedGuestItem.Content.ToString();
    if (int.TryParse(contentValue, out int parsed))
    {
        guestCount = parsed;
    }
}