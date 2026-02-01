# Yeni Özellikler - Masa Sipariş ve Ödeme Sistemi

Bu güncellemede KobiPOS sistemine aşağıdaki özellikler eklenmiştir:

## ✨ Eklenen Özellikler

### 1. Masa Sipariş Ekranı (OrderView)

**Özellikler:**
- Kategorilere göre ürün listesi
- Ürün seçimi ve siparişe ekleme
- Sipariş kalemlerinde:
  - Adet artırma/azaltma (+/-)
  - Ürün silme (X)
  - Not ekleme (örn: "Az şekerli")
- Otomatik hesaplama:
  - Ara Toplam
  - KDV (%20)
  - Genel Toplam
- **Kaydet** butonu: Siparişi kaydet ve masalar ekranına dön
- **Hesap Kapat** butonu: Ödeme ekranına geç

### 2. Ödeme Ekranı (CheckoutView)

**Özellikler:**
- Sipariş özeti ve toplam tutar görüntüleme
- İndirim işlemleri:
  - Yüzde indirimi (%)
  - Tutar indirimi (₺)
- Ödeme türü seçimi:
  - 🟢 Nakit
  - 🟡 Kredi Kartı
  - 🔵 Yemek Kartı
- Nakit ödeme için:
  - Alınan tutar girişi
  - Otomatik para üstü hesaplama
- **Ödemeyi Tamamla** butonu:
  - Siparişi veritabanına kaydet
  - Masayı "Boş" duruma getir
  - Ana ekrana dön

### 3. Adisyon Yazdırma

**Özellikler:**
- 80mm termal yazıcı formatında metin oluşturma
- Adisyon içeriği:
  - Firma bilgileri (Kobi Bilişim)
  - Masa numarası ve tarih
  - Kullanıcı bilgisi
  - Sipariş kalemleri ve notlar
  - Ara toplam, KDV, indirim
  - Toplam tutar
  - Ödeme türü
  - Para üstü (nakit için)
- **Adisyon Yazdır** butonu:
  - Adisyonu .txt dosyası olarak kaydet
  - Dosyayı otomatik olarak aç

## 🗂️ Veritabanı Güncellemeleri

### Orders Tablosu - Yeni Alanlar
- `SubTotal` - Ara toplam
- `TaxAmount` - KDV tutarı
- `DiscountPercent` - İndirim yüzdesi
- `DiscountAmount` - İndirim tutarı
- `Notes` - Sipariş notu

### OrderDetails Tablosu - Yeni Alanlar
- `ProductName` - Ürün adı
- `LineTotal` - Satır toplamı

## 📋 Kullanım Senaryoları

### Senaryo 1: Sipariş Alma
1. "Masalar" ekranında bir masaya tıklayın
2. Sipariş ekranı açılır
3. Kategorilerden ürün seçin (örn: 2x Türk Kahvesi, 1x Cheesecake)
4. İsterseniz ürüne not ekleyin (örn: "Az şekerli")
5. **Kaydet** butonuna basın
6. Sipariş kaydedilir ve masa "Dolu" duruma geçer
7. Masa üzerinde toplam tutar görünür

### Senaryo 2: Hesap Kapatma
1. Dolu bir masaya tıklayın
2. Sipariş ekranında **Hesap Kapat** butonuna basın
3. Ödeme ekranı açılır
4. İsterseniz indirim uygulayın (örn: %10)
5. Ödeme türünü seçin (Nakit, Kredi Kartı, Yemek Kartı)
6. Nakit ise alınan tutarı girin (para üstü otomatik hesaplanır)
7. **Ödemeyi Tamamla** butonuna basın
8. Sipariş tamamlanır, masa "Boş" duruma geçer
9. İsterseniz **Adisyon Yazdır** butonuna basarak adisyonu kaydedin

## 🎨 UI/UX Özellikleri

- **Touch-friendly**: 80x80px minimum buton boyutu
- **Kategori seçimi**: Üst kısımda yatay scroll
- **Renkli durumlar**: 
  - Yeşil (Boş masa, ekleme butonları)
  - Kırmızı (Dolu masa, silme butonları)
  - Sarı (İndirim, Kredi Kartı)
  - Mavi (Yemek Kartı)
- **Responsive tasarım**: Grid layout ile esnek yapı
- **Büyük fontlar**: Önemli tutarlar için kolay okunabilirlik

## 🔧 Teknik Detaylar

- **MVVM Pattern**: OrderViewModel, CheckoutViewModel
- **INotifyPropertyChanged**: Otomatik UI güncellemesi
- **RelayCommand**: Buton komutları
- **SQLite**: Veri saklama
- **Türkçe dil desteği**: Tüm arayüz Türkçe
- **Para birimi**: ₺ (TL) formatı

## 📁 Dosya Yapısı

```
KobiPOS/
├── Models/
│   ├── Order.cs (güncellendi)
│   ├── OrderDetail.cs (güncellendi)
│   └── OrderItem.cs (yeni)
├── ViewModels/
│   ├── OrderViewModel.cs (yeni)
│   ├── CheckoutViewModel.cs (yeni)
│   └── TablesViewModel.cs (güncellendi)
├── Views/
│   ├── OrderView.xaml (yeni)
│   ├── CheckoutView.xaml (yeni)
│   └── TablesView.xaml (güncellendi)
└── Services/
    └── DatabaseService.cs (güncellendi)
```

## 🚀 Gelecek İyileştirmeler

- Sipariş durumu takibi (Bekliyor → Hazırlanıyor → Hazır → Servis Edildi)
- Mutfak ekranı için sipariş bildirimi
- Masa transferi (siparişi başka masaya taşı)
- Masa birleştirme
- Sipariş geçmişi görüntüleme
- Split payment (birden fazla ödeme türü)
- Direkt yazıcıya yazdırma desteği

---

**Not:** Firma bilgileri (Kobi Bilişim, 0552 165 04 35, www.kobibilisim.com) tüm ekranlarda korunmuştur.
