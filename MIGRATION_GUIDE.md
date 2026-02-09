# Migration Rehberi

## Durum Kontrolü

Eğer daha önce migration oluşturduysanız ve veritabanında migration geçmişi varsa:

### Seçenek 1: Yeni Migration Ekle (Önerilen)
```bash
dotnet ef migrations add FixWalletTransactionRelationships
dotnet ef database update
```

### Seçenek 2: Veritabanını Sıfırla (Dikkat: Tüm veriler silinir!)
```bash
# Önce migration'ları sil
Remove-Item -Recurse -Force Migrations\*

# Yeni migration oluştur
dotnet ef migrations add InitialCreate

# Veritabanını sıfırla ve oluştur
dotnet ef database drop --force
dotnet ef database update
```

### Seçenek 3: Mevcut Migration'ı Düzenle
Eğer migration henüz production'a gitmediyse, mevcut migration dosyasını düzenleyebilirsiniz.

## Yapılan Düzeltmeler

1. **Wallet.Transactions** → **Wallet.OutgoingTransactions** olarak değiştirildi
2. **TransactionConfiguration** düzeltildi:
   - FromWallet → Wallet.OutgoingTransactions ile ilişkilendirildi
   - ToWallet → Navigation collection yok (sadece foreign key)

Bu değişiklikler sayesinde EF Core relationship hatası çözüldü.

