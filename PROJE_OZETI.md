# KobiPOS - Proje Özeti ve Kullanım Kılavuzu

## 📝 Genel Bakış

KobiPOS, cafe ve restoranlar için profesyonel bir masaüstü yönetim sistemidir. WPF (Windows Presentation Foundation) ve .NET 8.0 kullanılarak geliştirilmiştir.

## 🎯 Ana Özellikler

### 1. Kullanıcı Yönetimi
- **3 Rol Seviyesi:**
  - **Admin:** Tüm sistem ayarlarına erişim
  - **Kasiyer:** Kasa ve ödeme işlemleri
  - **Garson:** Sipariş alma ve masa yönetimi

- **Varsayılan Kullanıcılar:**
  ```
  Admin:   username: admin    password: admin123
  Garson:  username: garson1  password: garson123
  Kasiyer: username: kasiyer1 password: kasiyer123
  ```

### 2. Lisans Sistemi
- **Deneme Sürümü:** 7 gün ücretsiz kullanım
- **Tam Lisans:** 1 yıllık aktivasyon
- **Hardware-Based:** CPU ve Motherboard ID ile kilitleme
- **Offline Aktivasyon:** İnternet bağlantısı gerektirmez

### 3. Masa Yönetimi
- 10 adet örnek masa
- Görsel durum göstergesi (Boş/Dolu/Rezerve)
- Masa açma/kapama işlemleri
- Renkli durum kodlaması:
  - 🟢 Yeşil: Boş
  - 🔴 Kırmızı: Dolu
  - 🟡 Sarı: Rezerve

### 4. Ürün ve Kategori Yönetimi
- **5 Kategori:**
  1. Kahveler
  2. Soğuk İçecekler
  3. Tatlılar
  4. Ana Yemekler
  5. Atıştırmalıklar

- **20 Örnek Ürün** (fiyatlar, stok bilgileri ile)
- Kategori bazlı filtreleme
- Fiyat ve stok takibi

### 5. Raporlama
- Günlük satış raporları
- Kategori bazlı analizler
- Kullanıcı performans raporları
- Gelecek versiyonlarda: Grafik ve Excel çıktısı

## 🏗️ Mimari Yapı

### MVVM Pattern
```
KobiPOS/
├── Models/              # Veri modelleri
├── Views/               # XAML kullanıcı arayüzleri
├── ViewModels/          # İş mantığı ve veri bağlama
├── Services/            # Veritabanı, lisans, donanım servisleri
└── Helpers/             # Yardımcı sınıflar
```

### Teknolojiler
- **Framework:** .NET 8.0 (Windows)
- **UI Framework:** WPF (Windows Presentation Foundation)
- **Database:** SQLite (yerleşik, kurulum gerektirmez)
- **Pattern:** MVVM (Model-View-ViewModel)
- **Güvenlik:** SHA256 şifreleme

## 💾 Veritabanı Yapısı

### Tablolar
1. **Users** - Kullanıcı bilgileri ve roller
2. **Tables** - Masa bilgileri ve durumları
3. **Categories** - Ürün kategorileri
4. **Products** - Ürün bilgileri, fiyatlar, stok
5. **Orders** - Sipariş başlıkları
6. **OrderDetails** - Sipariş detayları
7. **Stock** - Stok hareketleri
8. **Licenses** - Lisans bilgileri
9. **AppSettings** - Uygulama ayarları

### Veritabanı Konumu
```
[UygulamaKlasörü]\Database\kobipos.db
```

## 🔒 Güvenlik Özellikleri

1. **Şifre Güvenliği:**
   - SHA256 hash ile şifreleme
   - Veritabanında düz metin saklanmaz

2. **Lisans Güvenliği:**
   - Hardware ID bazlı kilitleme
   - Benzersiz lisans anahtarları
   - Donanım değişikliği kontrolü

3. **SQL Injection Koruması:**
   - Parametreli sorgular
   - ORM benzeri güvenli erişim

## 📱 Kullanıcı Arayüzü

### Ana Ekranlar

1. **Giriş Ekranı (LoginWindow)**
   - Kullanıcı adı ve şifre girişi
   - Hata mesajları
   - Kobi Bilişim firma bilgileri

2. **Ana Panel (MainWindow)**
   - Üst başlık: Başlık ve kullanıcı bilgisi
   - Sol menü: Hızlı erişim butonları
   - İçerik alanı: Dinamik görünümler
   - Alt bilgi: Firma iletişim bilgileri

3. **Masalar (TablesView)**
   - Görsel masa düzeni
   - Durum bazlı renklendirme
   - Masa açma/kapama butonları

4. **Ürünler (ProductView)**
   - Kategori filtreleme
   - Ürün listesi (DataGrid)
   - Fiyat ve stok bilgileri

5. **Raporlar (ReportView)**
   - Günlük özet
   - Satış istatistikleri

6. **Lisans (LicenseView)**
   - Mevcut lisans durumu
   - Hardware ID görüntüleme ve kopyalama
   - Lisans aktivasyon formu

7. **Destek (SupportView)**
   - Firma bilgileri
   - WhatsApp ve web sitesi linkleri
   - Versiyon bilgisi

## 🎨 Tema ve Tasarım

### Renk Paleti
- **Primary (Mavi):** #2196F3
- **Success (Yeşil):** #4CAF50
- **Danger (Kırmızı):** #F44336
- **Warning (Sarı):** #FFC107
- **Accent (Turuncu):** #FF9800

### Özellikler
- Modern, minimalist tasarım
- Büyük, dokunmatik ekran uyumlu butonlar
- Responsive layout
- Tutarlı renk kodlaması

## 📋 Kullanım Senaryoları

### Senaryo 1: Masa Açma
1. Admin/Garson olarak giriş yap
2. "Masalar" menüsüne git
3. Boş (yeşil) bir masaya tıkla
4. "Aç" butonuna bas
5. Masa "Dolu" (kırmızı) olarak işaretlenir

### Senaryo 2: Ürün Görüntüleme
1. "Ürünler" menüsüne git
2. Üstteki kategorilerden birini seç
3. O kategorideki ürünleri listede gör
4. Fiyat ve stok bilgilerini kontrol et

### Senaryo 3: Lisans Aktivasyonu
1. "Lisans" menüsüne git
2. Hardware ID'yi kopyala
3. Kobi Bilişim'e (0552 165 04 35) ilet
4. Aldığın lisans anahtarını gir
5. Müşteri adını yaz
6. "Lisansı Aktive Et" butonuna bas

## 🔧 Geliştirme Notları

### Proje Yapısı
```
KobiPOS.sln                    # Visual Studio Solution
├── KobiPOS/                   # Ana proje
│   ├── KobiPOS.csproj        # Proje dosyası
│   ├── App.xaml              # Uygulama kaynakları ve stilleri
│   ├── Models/               # 9 model sınıfı
│   ├── ViewModels/           # 8 ViewModel sınıfı
│   ├── Views/                # 8 View (XAML + CS)
│   ├── Services/             # 4 servis sınıfı
│   ├── Helpers/              # 3 yardımcı sınıf
│   └── Database/             # SQLite DB (runtime'da oluşur)
└── README.md                  # Kullanım kılavuzu
```

### NuGet Paketleri
- **Microsoft.Data.Sqlite** (8.0.0) - SQLite veritabanı
- **System.Data.SQLite.Core** (1.0.118) - SQLite core
- **System.Management** (8.0.0) - Hardware ID için

### Önemli Sınıflar

#### Services
- **DatabaseService:** Singleton pattern, SQLite işlemleri
- **LicenseService:** Lisans doğrulama ve aktivasyon
- **HardwareService:** CPU ve Motherboard ID alma
- **PrintService:** Adisyon formatı oluşturma

#### Helpers
- **RelayCommand:** ICommand implementasyonu
- **PasswordHelper:** SHA256 hash işlemleri
- **ValidationHelper:** Veri doğrulama

## 🚀 Gelecek Geliştirmeler

### v2.0 Planlanan Özellikler
- [ ] Sipariş yönetimi (sipariş oluşturma, güncelleme)
- [ ] Masa birleştirme ve transfer
- [ ] Mutfak ekranı modülü
- [ ] Gelişmiş raporlama (grafikler, Excel export)
- [ ] Stok kritik seviye uyarıları
- [ ] Çoklu ödeme tipi (Nakit, Kredi Kartı, Yemek Kartı)
- [ ] İndirim ve ikram işlemleri
- [ ] Vardiya yönetimi
- [ ] Kullanıcı ekleme/düzenleme/silme UI
- [ ] Ürün ekleme/düzenleme/silme UI
- [ ] Termal yazıcı entegrasyonu
- [ ] QR kod ile adisyon
- [ ] Açık/Kapalı tema seçeneği

### Teknik İyileştirmeler
- [ ] Unit testler
- [ ] Integration testler
- [ ] Logging sistemi (NLog/Serilog)
- [ ] Otomatik veritabanı yedekleme
- [ ] Crash rapor sistemi
- [ ] Performans optimizasyonları
- [ ] Çoklu dil desteği (EN, TR)

## 📞 Destek ve İletişim

**Kobi Bilişim**
- **Telefon:** 0552 165 04 35
- **WhatsApp:** 0552 165 04 35
- **Web:** www.kobibilisim.com

**Çalışma Saatleri:** Hafta içi 09:00 - 18:00

## 📄 Lisans

© 2024 Kobi Bilişim. Tüm hakları saklıdır.

---

**KobiPOS v1.0.0** - Profesyonel Cafe & Restoran Yönetim Sistemi
