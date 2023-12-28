# NLP – Karakter’den Numara’ya Dönüşüm Çalışması


# Proje İçeriği

1 adet string input alan MVC Web API metodu
geliştirilecektir. İlgili string input JSON formatında API
metoduna POST işlemi ile gönderilecektir. Testler
sırasında aşağıdaki JSON sorgusu için gönderim
yapılacaktır.

{ "UserText": "yüz bin lira kredi kullanmak istiyorum" }

POST ile gönderilen bu text verisi, API metodu içinde
işlenerek cümle içindeki yazı ile yazılmış rakamlar,
numeric formata dönüştürülecektir. Yani yukarıdaki
örnekte belirtilen text aşağıdaki output haline
dönüştürülecektir.

{ "Output": "100000 lira kredi kullanmak istiyorum" }
# Kullanılan Teknolojiler

Net core 7.0
C# • MVC Web API

