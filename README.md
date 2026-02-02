# KobiPOS - Cafe & Restoran Yönetim Sistemi

Profesyonel bir cafe ve restoran yönetim sistemi masaüstü uygulaması.

> 🛠️ **Sorun mu yaşıyorsunuz?** Format hatası alıyorsanız [Manuel Temizlik Rehberi](MANUEL_TEMIZLIK.md)'ni inceleyin.

## 📋 Özellikler

- **Kullanıcı Yönetimi:** Admin, Kasiyer ve Garson rollerinde kullanıcı girişi
- **Masa Yönetimi:** Masaların durumunu görüntüleme ve yönetme (Boş/Dolu/Rezerve)
- **Ürün Yönetimi:** Kategoriler ve ürünler yönetimi
- **Sipariş Takibi:** Siparişlerin alınması ve takibi
- **Lisans Sistemi:** 7 günlük deneme sürümü ve 1 yıllık lisans aktivasyonu
- **Raporlama:** Satış ve performans raporları
- **Adisyon Çıktısı:** Termal yazıcı desteği

## 🏢 Firma Bilgileri

- **Firma Adı:** Kobi Bilişim
- **Telefon:** 0552 165 04 35
- **Web Sitesi:** www.kobibilisim.com
- **WhatsApp:** 0552 165 04 35

## 💻 Gereksinimler

### Geliştirme Ortamı
- **İşletim Sistemi:** Windows 10/11
- **Visual Studio:** 2022 (Community/Professional/Enterprise)
- **.NET SDK:** .NET 8.0 veya üzeri
- **Workload:** .NET desktop development

### Çalışma Zamanı Gereksinimleri
- **İşletim Sistemi:** Windows 10/11
- **.NET Desktop Runtime:** 8.0 veya üzeri

## 🚀 Kurulum ve Derleme

### 1. Visual Studio Kurulumu

Visual Studio 2022'yi indirin ve kurun:
1. [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) adresinden indirin
2. Kurulum sırasında **.NET desktop development** workload'ını seçin
3. İsteğe bağlı olarak **Desktop development with C++** seçeneğini de ekleyebilirsiniz

### 2. Projeyi Açma

```bash
# Repoyu klonlayın
git clone https://github.com/BYIRMAK/kobiadisyon.git
cd kobiadisyon

# Visual Studio ile açın
start KobiPOS.sln
```

Alternatif olarak:
- Visual Studio'yu açın
- `File` → `Open` → `Project/Solution`
- `KobiPOS.sln` dosyasını seçin

### 3. NuGet Paketlerini Geri Yükleme

Visual Studio otomatik olarak NuGet paketlerini geri yükleyecektir. Manuel olarak yapmak isterseniz:

```bash
# Terminal veya Package Manager Console'da
dotnet restore
```

### 4. Projeyi Derleme

**Visual Studio'da:**
- `Build` → `Build Solution` (veya `Ctrl+Shift+B`)

**Terminal'de:**
```bash
dotnet build KobiPOS/KobiPOS.csproj --configuration Release
```

### 5. Projeyi Çalıştırma

**Visual Studio'da:**
- `Debug` → `Start Debugging` (veya `F5`)
- Veya `Debug` → `Start Without Debugging` (veya `Ctrl+F5`)

**Terminal'de:**
```bash
dotnet run --project KobiPOS/KobiPOS.csproj
```

## 📦 Tek .EXE Dosyası Olarak Yayınlama

### Visual Studio Kullanarak

1. Solution Explorer'da `KobiPOS` projesine sağ tıklayın
2. `Publish` seçeneğini seçin
3. `Folder` hedefini seçin
4. Hedef konumu belirleyin (örn: `bin\Release\publish`)
5. Configuration:
   - **Target Runtime:** `win-x64` veya `win-x86`
   - **Deployment Mode:** `Self-contained`
   - **Produce single file:** ✓ (işaretleyin)
   - **Enable ReadyToRun compilation:** ✓ (isteğe bağlı, performans için)
6. `Publish` butonuna tıklayın

### Komut Satırı ile

**Tek dosya olarak (Self-contained):**
```bash
dotnet publish KobiPOS/KobiPOS.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishReadyToRun=true
```

**Framework-dependent (daha küçük dosya boyutu):**
```bash
dotnet publish KobiPOS/KobiPOS.csproj -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```

Yayınlanan dosyalar şurada bulunur:
```
KobiPOS\bin\Release\net8.0-windows\win-x64\publish\
```

## 🔧 İlk Kurulum ve Kullanım

### 1. İlk Çalıştırma

Uygulama ilk çalıştırıldığında:
- SQLite veritabanı otomatik oluşturulur (`Database/kobipos.db`)
- Varsayılan kullanıcılar ve örnek veriler yüklenir
- 7 günlük deneme süresi başlar

### 2. Varsayılan Kullanıcı Girişi

**Admin Kullanıcısı:**
- Kullanıcı Adı: `admin`
- Şifre: `admin123`

**Garson Kullanıcısı:**
- Kullanıcı Adı: `garson1`
- Şifre: `garson123`

**Kasiyer Kullanıcısı:**
- Kullanıcı Adı: `kasiyer1`
- Şifre: `kasiyer123`

⚠️ **Güvenlik Uyarısı:** İlk girişten sonra mutlaka şifreleri değiştirin!

### 3. Lisans Aktivasyonu

#### Deneme Sürümü
- Uygulama ilk çalıştırıldığında 7 gün ücretsiz deneme süresi başlar
- Her açılışta kalan gün sayısı gösterilir
- Deneme süresi bitiminde uygulama salt okunur moda geçer

#### Tam Lisans Aktivasyonu

1. **Hardware ID Alma:**
   - Uygulamada `Lisans` menüsüne gidin
   - Hardware ID'yi kopyalayın
   - Bu ID'yi Kobi Bilişim'e iletin (0552 165 04 35 veya WhatsApp)

2. **Lisans Anahtarı Alma:**
   - Kobi Bilişim size benzersiz bir lisans anahtarı gönderecektir
   - Format: `XXXXX-XXXXX-XXXXX-XXXXX`

3. **Aktivasyon:**
   - `Lisans` menüsünde `Müşteri Adı` alanını doldurun
   - `Lisans Anahtarı` alanına aldığınız anahtarı girin
   - `Lisansı Aktive Et` butonuna tıklayın
   - Başarılı aktivasyon sonrası uygulamayı yeniden başlatın

## 📊 Veritabanı

Uygulama SQLite kullanır ve veritabanı dosyası şurada bulunur:
```
[UygulamaKlasörü]\Database\kobipos.db
```

### Veritabanı Yedekleme

Düzenli olarak `kobipos.db` dosyasını yedekleyin:
```bash
# Windows'ta
copy "Database\kobipos.db" "Backup\kobipos_backup_[tarih].db"
```

### Veritabanı Sıfırlama

Veritabanını sıfırlamak için:
1. Uygulamayı kapatın
2. `Database\kobipos.db` dosyasını silin
3. Uygulamayı yeniden başlatın (otomatik yeniden oluşturulur)

## 🔍 Sorun Giderme

### Uygulama Açılmıyor

**Hata: ".NET Desktop Runtime bulunamadı"**
- [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) indirip kurun

**Hata: "Veritabanı oluşturulamadı"**
- Uygulamanın yazma izinlerine sahip olduğundan emin olun
- Antivirüs yazılımınızın engellemediğinden emin olun

### Giriş Yapamıyorum

- Caps Lock kapalı olduğundan emin olun
- Varsayılan kullanıcı bilgilerini kullanın (admin/admin123)
- Veritabanını sıfırlayın (yukarıdaki talimatları izleyin)

### Lisans Aktivasyonu Başarısız

- Lisans anahtarının doğru formatta olduğundan emin olun
- İnternet bağlantısı olduğundan emin olun
- Hardware ID'nin değişmediğinden emin olun
- Kobi Bilişim ile iletişime geçin: 0552 165 04 35

### Performans Sorunları

- Veritabanı dosyasının çok büyük olup olmadığını kontrol edin
- Eski kayıtları arşivleyin veya silin
- Uygulamayı yeniden başlatın

### "Input string was not in a correct format" Hatası

Bu hata genellikle Visual Studio'nun eski DLL cache kullanmasından kaynaklanır.

**ÇÖZÜM:** [Manuel Temizlik Rehberi](MANUEL_TEMIZLIK.md) - Adım adım temizlik talimatları

**Hızlı Çözüm:**
1. Visual Studio'yu kapat
2. `bin`, `obj`, `.vs` klasörlerini sil
3. Visual Studio'yu aç
4. `Build` → `Clean Solution`
5. `Build` → `Rebuild Solution`
6. Uygulamayı yeniden çalıştır

Detaylı adımlar ve debug talimatları için manuel temizlik rehberini inceleyin.

## 📱 İletişim ve Destek

Teknik destek için:

- **Telefon:** 0552 165 04 35
- **WhatsApp:** 0552 165 04 35
- **Web:** www.kobibilisim.com
- **E-posta:** info@kobibilisim.com (eğer varsa)

Çalışma Saatleri: Hafta içi 09:00 - 18:00

## 📝 Lisans

© 2024 Kobi Bilişim. Tüm hakları saklıdır.

Bu yazılım Kobi Bilişim'in telif hakkıdır. Yetkisiz kopyalama, dağıtım veya kullanım yasaktır.

## 🔄 Versiyon Geçmişi

### v1.0.0 (İlk Sürüm)
- ✅ Kullanıcı yönetimi (Admin/Kasiyer/Garson)
- ✅ Masa yönetimi
- ✅ Ürün ve kategori yönetimi
- ✅ Lisans sistemi (7 gün deneme + 1 yıl tam lisans)
- ✅ Temel raporlama
- ✅ Adisyon çıktısı
- ✅ Firma bilgileri ve destek ekranı

## 🎯 Gelecek Özellikler

- [ ] Gelişmiş raporlama ve analizler
- [ ] Stok yönetimi ve uyarıları
- [ ] Mutfak ekranı modülü
- [ ] Masa birleştirme ve transfer
- [ ] Çoklu ödeme tipi desteği
- [ ] Fatura ve e-Fatura entegrasyonu
- [ ] QR kod ile sipariş
- [ ] Mobil uygulama desteği

---

**KobiPOS** - Profesyonel Cafe & Restoran Yönetim Sistemi

Geliştirici: **Kobi Bilişim** | www.kobibilisim.com | 0552 165 04 35
