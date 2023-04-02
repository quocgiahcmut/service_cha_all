Vì chương trình có thêm các kết nối đến Opc Ua Server 
Mà Opc Ua Server thì không phải lúc nào cũng có để test

=> Nên tắt OpcUaClient đi để chương trình không tìm kiếm và cố gắn
   kết nối với OpcUaServer

Cách tắt: 
1) Program.cs => Tìm đoạn code dưới đây: 

services.AddHostedService<OpcUaWorker>(provider =>
        {
            var ow = new OpcUaWorker(
                provider.GetRequiredService<ILogger<OpcUaWorker>>(),
                provider.GetRequiredService<IBusControl>(),
                serviceProvider.GetRequiredService<LargeOneUaClientHelper>(),
                serviceProvider.GetRequiredService<LargeTwoUaClientHelper>(),
                serviceProvider.GetRequiredService<LargeThreeUaClientHelper>());

            return ow;
        });

Hiện tại đang nằm ở dòng 28

2) Bôi đen và nhấn tổ hợp phím "Ctrl + Shift + /" 
   để comment phần code đó lại

Lúc nào OpcUaWorker sẽ không đc khởi tạo và không chạy 3 OpcUaClient