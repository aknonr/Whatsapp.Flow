// MongoDB başlatma scripti
db = db.getSiblingDB('WhatsappFlowDb');

// Koleksiyonları oluştur
db.createCollection('Flows');
db.createCollection('FlowStates');
db.createCollection('FlowTemplates');
db.createCollection('Tenants');
db.createCollection('Users');

// Index'ler oluştur
db.Flows.createIndex({ "TenantId": 1 });
db.Flows.createIndex({ "IsActive": 1 });
db.Flows.createIndex({ "TenantId": 1, "IsActive": 1 });

db.FlowStates.createIndex({ "TenantId": 1 });
db.FlowStates.createIndex({ "UserPhoneNumber": 1 });
db.FlowStates.createIndex({ "FlowId": 1 });

db.Users.createIndex({ "TenantId": 1 });
db.Users.createIndex({ "Email": 1 }, { unique: true });

db.Tenants.createIndex({ "PhoneNumber": 1 }, { unique: true });

print('MongoDB başlatma tamamlandı!'); 