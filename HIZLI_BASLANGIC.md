# KobiPOS - Hızlı Başlangıç Kılavuzu

## ⚡ 5 Dakikada Başla

### 1️⃣ Gereksinimleri Kontrol Et

**Windows 10/11** bilgisayarınızda olmalı:
- [ ] .NET 8.0 Desktop Runtime yüklü mü?
  - İndirmek için: https://dotnet.microsoft.com/download/dotnet/8.0
  - "Desktop Runtime" seçeneğini indirin

### 2️⃣ Uygulamayı İndir veya Derle

#### Seçenek A: Hazır .exe İndir (Önerilir)
1. Releases sayfasından son sürümü indir
2. .zip dosyasını aç
3. `KobiPOS.exe` dosyasına çift tıkla

#### Seçenek B: Kaynak Koddan Derle
```bash
# 1. Repoyu klonla
git clone https://github.com/BYIRMAK/kobiadisyon.git
cd kobiadisyon

# 2. Visual Studio 2022 ile aç
start KobiPOS.sln

# 3. F5'e bas veya "Start" butonuna tıkla
```

### 3️⃣ İlk Giriş

**Varsayılan Admin Bilgileri:**
```
Kullanıcı Adı: admin
Şifre: admin123
```

### 4️⃣ Sistemi Keşfet

1. **Masalar:** 10 adet örnek masa görüntüle
2. **Ürünler:** 20 ürünü kategoriler ile gör
3. **Lisans:** Hardware ID'ni kontrol et
4. **Destek:** Firma bilgilerini gör

### 5️⃣ Lisans Aktive Et (İsteğe Bağlı)

7 günlük deneme süresi var, ancak tam lisans için:

1. **Lisans** menüsüne git
2. **Hardware ID**'ni kopyala
3. **0552 165 04 35** numarasından iletişime geç
4. Aldığın **Lisans Anahtarı**'nı gir
5. **Aktive Et** butonuna bas

## 🎓 Temel İşlemler

### Masa Açma
```
Masalar → Yeşil Masa → "Aç" Butonu
```

### Masa Kapatma
```
Masalar → Kırmızı Masa → "Kapat" Butonu
```

### Ürünleri Görüntüleme
```
Ürünler → Kategori Seç → Liste Görüntüle
```

### Kullanıcı Değiştirme
```
Sağ Üst → "Çıkış Yap" → Yeni Kullanıcı ile Giriş
```

## ❓ Sık Sorulan Sorular

**S: "Input string was not in a correct format" hatası alıyorum**
C: Bu Visual Studio cache sorunu olabilir. [Manuel Temizlik Rehberi](MANUEL_TEMIZLIK.md)'ni takip edin.

**S: Şifremi unuttum, ne yapmalıyım?**
C: Veritabanını sıfırlayın (`Database/kobipos.db` dosyasını silin) veya destek ile iletişime geçin.

**S: Deneme süresi doldu, ne olur?**
C: Uygulama salt okunur moda geçer. Tam lisans almak için Kobi Bilişim ile iletişime geçin.

**S: Uygulamayı başka bilgisayara taşıyabilir miyim?**
C: Evet, ancak yeni bir lisans anahtarı gerekir (Hardware ID farklı olacaktır).

**S: Veritabanı nerede saklanıyor?**
C: `[UygulamaKlasörü]/Database/kobipos.db` konumunda.

## 📞 Yardıma mı İhtiyacınız Var?

**Kobi Bilişim**
📱 0552 165 04 35
💬 WhatsApp: 0552 165 04 35
🌐 www.kobibilisim.com

Hafta içi 09:00 - 18:00

---

İyi çalışmalar! 🎉
