# 🛠️ MANUEL TEMİZLİK VE YENİDEN DERLEME REHBERİ

## 📋 SORUN

**HATA:** "Input string was not in a correct format."

**NEDEN:** Visual Studio eski DLL cache kullanıyor. Kod doğru olmasına rağmen, eski binary dosyaları çalıştırılıyor.

**ÇÖZÜM:** Bu rehberi takip ederek tüm cache ve geçici dosyaları temizleyin.

---

## ✅ ADIM ADIM TEMİZLİK TALİMATLARI

### 1️⃣ Visual Studio'yu KAPATIN

Tüm Visual Studio pencerelerini kapatın. Arka planda çalışan Visual Studio süreçlerini de kontrol edin:

1. `Ctrl + Shift + Esc` (Görev Yöneticisi)
2. `devenv.exe` veya `MSBuild.exe` süreçleri varsa sonlandırın

### 2️⃣ Proje Klasörlerini Temizleyin

Windows Explorer'da proje klasörünüze gidin:
```
C:\Users\[KULLANICI_ADI]\Desktop\kobiadisyon-main\
```

**ŞU KLASÖRLERI TAMAMEN SİLİN:**
```
KobiPOS\bin      (TAMAMEN SİL)
KobiPOS\obj      (TAMAMEN SİL)  
.vs              (gizli klasör - TAMAMEN SİL)
```

**Gizli klasörleri görmek için:**
- Windows Explorer → `Görünüm` sekmesi → `Gizli öğeler` ✅ (işaretleyin)

### 3️⃣ Yeni Kopya İndirin (ÖNERİLEN)

En garantili yöntem yeni bir kopya indirmektir:

1. https://github.com/BYIRMAK/kobiadisyon adresine gidin
2. `Code` → `Download ZIP` tıklayın
3. ZIP'i yeni bir klasöre çıkartın: `kobiadisyon-FRESH`
4. Bu yeni klasörle devam edin

**VEYA** mevcut klasörü kullanmaya devam edin (adım 2'yi tamamladıysanız).

### 4️⃣ Solution'ı Açın

Visual Studio 2022'yi açın ve solution dosyasını açın:
```
kobiadisyon-FRESH\KobiPOS.sln
```

### 5️⃣ NuGet Paketlerini Geri Yükleyin

Visual Studio'da:
1. `Solution Explorer`'da Solution'a (en üstteki öğe) **sağ tıklayın**
2. `Restore NuGet Packages` seçeneğini seçin
3. `Output` penceresinde "Restore completed" mesajını bekleyin

**Alternatif:** Package Manager Console'da:
```powershell
dotnet restore
```

### 6️⃣ Clean + Rebuild Yapın

**Visual Studio'da:**
1. `Build` → `Clean Solution` (temizle)
2. Birkaç saniye bekleyin
3. `Build` → `Rebuild Solution` (`Ctrl + Shift + B`)

**Package Manager Console'da:**
```powershell
dotnet clean
dotnet build --no-incremental
```

### 7️⃣ Output Penceresini Kontrol Edin

Build tamamlandığında `Output` penceresinde şunu görmelisiniz:
```
========== Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========
```

**EĞER HATA VARSA:**
- Hata mesajlarını okuyun
- NuGet restore işlemini tekrarlayın
- Visual Studio'yu yeniden başlatıp tekrar deneyin

### 8️⃣ Uygulamayı Çalıştırın

**Debug mode:** `F5` (veya `Debug` → `Start Debugging`)

**Normal mode:** `Ctrl + F5` (veya `Debug` → `Start Without Debugging`)

---

## 🎯 TEST

Rezervasyon özelliğini test edin:

1. Giriş yapın (admin/admin123)
2. Sol menüden **Rezervasyonlar** sekmesine gidin
3. **➕ Yeni Rezervasyon** butonuna tıklayın
4. Formu doldurun:
   - Müşteri Adı: `Test Müşteri`
   - Telefon: `555 123 4567`
   - Kişi Sayısı: `4`
   - Rezervasyon Tarihi: Bugün veya gelecek bir tarih
   - Rezervasyon Saati: `19:00`
   - Masa: Herhangi bir masa seçin
5. **💾 Kaydet** butonuna tıklayın
6. **✅ "Rezervasyon başarıyla oluşturuldu!" mesajını görmelisiniz**

**EĞER HALA HATA ALIYORSANIZ:** Adım 9'a geçin.

---

## 🐛 HALA SORUN VARSA: DEBUG MODE

Eğer hala "Input string was not in a correct format" hatası alıyorsanız, breakpoint ile debug yapmalıyız:

### Debug Adımları

1. **Breakpoint Koyma:**
   - `Solution Explorer` → `KobiPOS` → `Views` → `Dialogs` → `AddReservationDialog.xaml.cs` dosyasını açın
   - **Satır 91** (`try` bloğunun başı) numarasının yanına tıklayın (kırmızı nokta belirecek)
   - Veya satıra tıklayıp `F9` tuşuna basın

2. **Debug Başlatma:**
   - `F5` ile uygulamayı debug modda çalıştırın
   - Rezervasyon ekleme formunu açın
   - Formu doldurun ve **Kaydet**'e tıklayın

3. **Kodda İlerleme:**
   - Kod breakpoint'te duracak
   - `F10` tuşu ile satır satır ilerleyin
   - **HANGİ SATIRDA** hata oluştuğunu not alın

4. **Değişkenleri İnceleme:**
   - Hata olan satırda fareyi değişkenlerin üzerine getirin
   - Değerleri kontrol edin
   - `Locals` penceresinde tüm değişkenleri görebilirsiniz

5. **Bilgi Toplama:**
   - Hata veren satır numarası
   - Hata mesajı (Exception Details)
   - Değişken değerleri
   - Screenshot alın ve destek ekibiyle paylaşın

---

## 💡 EK İPUÇLARI

### Cache Temizleme (Windows)

Bazen Visual Studio kullanıcı düzeyinde cache tutar:

```
%LOCALAPPDATA%\Microsoft\VisualStudio\
```

Bu klasördeki tüm geçici dosyaları silebilirsiniz (Visual Studio kapalıyken).

### MSBuild Binary Log

Detaylı build log almak için:

```bash
dotnet build /bl
```

Bu `msbuild.binlog` dosyası oluşturur. Bu dosyayı [MSBuild Structured Log Viewer](https://msbuildlog.com/) ile açabilirsiniz.

### Tamamen Temiz Başlangıç

En garantili yöntem:

1. Visual Studio'yu kapat
2. Tüm proje klasörünü sil
3. GitHub'dan yeni ZIP indir
4. Yeni klasöre çıkart
5. Visual Studio ile aç
6. Restore + Build

---

## 📞 DESTEK

Bu adımları denedikten sonra hala sorun yaşıyorsanız:

### İletişim Bilgileri

- **Telefon:** 0552 165 04 35
- **WhatsApp:** 0552 165 04 35
- **Web:** www.kobibilisim.com

### Destek Talebi İçin Gerekli Bilgiler

1. Hata mesajının tam metni
2. Hata veren kod satırı (debug ile bulunmuşsa)
3. Visual Studio versiyonu
4. Windows versiyonu
5. .NET SDK versiyonu (`dotnet --version` komutu ile)
6. Screenshot (varsa)

---

## ✅ ÖZETİ KONTROL LİSTESİ

Son bir kontrol:

- [ ] Visual Studio tamamen kapatıldı mı?
- [ ] `bin`, `obj`, `.vs` klasörleri silindi mi?
- [ ] Yeni ZIP indirildi mi (veya mevcut klasör temizlendi mi)?
- [ ] NuGet restore yapıldı mı?
- [ ] Clean + Rebuild yapıldı mı?
- [ ] Build başarılı oldu mu (0 failed)?
- [ ] Uygulama çalıştırıldı mı?
- [ ] Rezervasyon testi yapıldı mı?

**Tüm maddelere ✅ işareti koyabiliyorsanız, sorun çözülmüş olmalıdır!**

---

**KobiPOS** - Profesyonel Cafe & Restoran Yönetim Sistemi

Geliştirici: **Kobi Bilişim** | www.kobibilisim.com | 0552 165 04 35
