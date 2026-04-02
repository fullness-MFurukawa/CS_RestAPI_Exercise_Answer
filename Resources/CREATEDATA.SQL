CREATE DATABASE restapi_exercise ENCODING 'UTF8';

CREATE TABLE product_category (
id             INT         GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
category_uuid  UUID        NOT NULL UNIQUE DEFAULT gen_random_uuid(),
name           VARCHAR(20) DEFAULT NULL
);

CREATE TABLE product (
id            INT         GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
product_uuid  UUID        NOT NULL UNIQUE DEFAULT gen_random_uuid(),
name          VARCHAR(30) DEFAULT NULL,
price         INT         DEFAULT NULL,
category_id   INT         DEFAULT NULL,
CONSTRAINT product_ibfk_1 FOREIGN KEY (category_id) REFERENCES product_category (id)
);

CREATE INDEX idx_product_category_id ON product (category_id);

CREATE TABLE product_stock (
id          INT  GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
stock_uuid  UUID NOT NULL UNIQUE DEFAULT gen_random_uuid(),
stock       INT  DEFAULT NULL,
product_id  INT  DEFAULT NULL,
CONSTRAINT product_stock_ibfk_1 FOREIGN KEY (product_id) REFERENCES product (id)
);

CREATE INDEX idx_product_stock_product_id ON product_stock (product_id);

INSERT INTO product_category (category_uuid, name) VALUES
('2f4d3e51-6f6b-11f0-954a-00155d1bd29a', '文房具'),
('2f5016b6-6f6b-11f0-954a-00155d1bd29a', '雑貨'),
('2f501b67-6f6b-11f0-954a-00155d1bd29a', 'パソコン周辺機器');

-- 文房具カテゴリ（category_id = 1）
INSERT INTO product (product_uuid, name, price, category_id) VALUES
('ac413f22-0cf1-490a-9635-7e9ca810e544','水性ボールペン(黒)',120,1),
('8f81a72a-58ef-422b-b472-d982e8665292','水性ボールペン(赤)',120,1),
('d952b98c-a1ea-478d-8380-3b90fde872ea','水性ボールペン(青)',120,1),
('9959e553-c9da-4646-bd85-8663a3541583','油性ボールペン(黒)',100,1),
('79023e82-9197-40a5-b236-26487f404be4','油性ボールペン(赤)',100,1),
('7dfd0fd0-0893-4d20-83ef-6f70aab0ab76','油性ボールペン(青)',100,1),
('dc7243af-c2ce-4136-bd5d-c6b28ee0a20a','蛍光ペン(黄)',130,1),
('83fbc81d-2498-4da6-b8c2-54878d3b67ff','蛍光ペン(赤)',130,1),
('ee4b3752-3fbd-45fc-afb5-8f37c3f701c9','蛍光ペン(青)',130,1),
('35cb51a7-df79-4771-9939-7f32c19bca45','蛍光ペン(緑)',130,1),
('e4850253-f363-4e79-8110-7335e4af45be','鉛筆(黒)',100,1),
('5ca7dbdf-0010-44c5-a001-e4c13c4fe3a1','鉛筆(赤)',100,1),
('fbc43b9b-90a9-4712-925c-4d66a2a30372','色鉛筆(12色)',400,1),
('4b3db238-8ada-49b4-bb60-1a034914e528','色鉛筆(48色)',1300,1);

-- 雑貨カテゴリ（category_id = 2）
INSERT INTO product (product_uuid, name, price, category_id) VALUES
('debdbd8c-5b48-4b1a-9697-98ba321ddd40','レザーネックレス',300,2),
('367197c5-32bd-479a-9102-c601145464c4','ワンタッチ開閉傘',3000,2),
('657578d2-8820-4490-a6ec-06d9c7cccd0f','金魚風呂敷',500,2),
('8c107894-4ebc-445b-9603-c9e8e6524f9d','折畳トートバッグ',600,2),
('2f8e074c-d0b1-441b-9dd4-6cf0ec570ce6','アイマスク',900,2),
('2fb9fe48-3520-47ef-9e1a-338db7152884','防水スプレー',500,2),
('f536311a-b9de-4873-a603-70953a2261be','キーホルダ',800,2);

-- パソコン周辺機器（category_id = 3）
INSERT INTO product (product_uuid, name, price, category_id) VALUES
('82014174-6785-4242-b307-a806fd1f8470','ワイヤレスマウス',900,3),
('ddd1e5ae-fb90-4a47-bb87-c91b305c7444','ワイヤレストラックボール',1300,3),
('aa5e07aa-06f9-4037-9755-e1de3c0ad4ac','有線光学式マウス',500,3),
('53cfa873-c86b-48bd-a68c-458d7bb5c844','光学式ゲーミングマウス',4800,3),
('376f7a75-cc99-4428-b35a-889bcb3c90af','有線ゲーミングマウス',3800,3),
('38c6e236-90ca-48a2-b427-acb9d834b591','USB有線式キーボード',1400,3),
('dc2e5a33-a2b7-4414-9a53-f9750e7da8ed','無線式キーボード',1900,3);


INSERT INTO product_stock (stock_uuid, stock, product_id) VALUES
('80ad9abf-5575-454a-bc7d-3068fa3077e8', 100, 1),
('684b2cb1-7c4d-4b56-9975-e7facd06ab23', 100, 2),
('f96f9d17-ae04-4205-820c-6895719eee78', 100, 3),
('a525cc53-dc0e-42d9-889f-b3fb2f1277d7', 100, 4),
('ff01a4b2-822e-462a-b200-2165bd07c618', 100, 5),
('d1628caa-3427-4374-b1fc-4cbd923b60b9', 100, 6),
('45a6b979-b0bc-400f-a450-88e554accb56', 100, 7),
('04266bd3-09ce-4a41-bb95-47a98c2f056b', 100, 8),
('0adcd322-cb08-4804-b8da-35cdf841b8c3', 100, 9),
('104f63a3-7c5c-4c74-84a1-c3bfa65a2bc4', 100, 10),
('3a3d43d8-2402-4517-98b9-72b6dea4b358', 100, 11),
('b3e88c96-f945-4e28-a8f2-b65ef9e40b87', 100, 12),
('1f00769c-c715-496b-b1e1-65df2305dff5', 100, 13),
('0a30b5eb-0ae0-44de-8b7f-4704d2948c79', 100, 14),
('148947c8-72f5-4a38-bfc7-587eed0d9011', 100, 15),
('d22db9d1-8ebf-4561-966f-18e0e17c966b', 100, 16),
('39a4f298-a87b-49c0-b1e9-42d29208f67e', 100, 17),
('5eff83e8-7bec-4c07-ac84-41c11f4fd868', 100, 18),
('6715c517-40d8-4d0f-9b79-979e84af4ee5', 100, 19),
('d4b326c7-7071-42d9-8183-bc49f23b7830', 100, 20),
('c4302573-6ac4-4252-aa42-a933d48e3748', 100, 21),
('e9749bee-c050-4622-a26f-30032a1b822b', 100, 22),
('65730621-b6bd-4bdd-811a-01d70a943766', 100, 23),
('92acb83f-da7c-43d0-90d2-a2609350b865', 100, 24),
('6805b314-0bd6-4f1d-adfe-c14d4ae6d3e7', 100, 25),
('68922056-8025-4bf3-bd18-37d7b6a6f4c7', 100, 26),
('d51a02f3-2a22-4bda-a302-7ae0b06d6046', 100, 27),
('6a0e83a1-9e6d-4c55-9d0c-4e0dd5957af2', 100, 28);

CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TABLE users (
    user_id       INT         GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_uuid     UUID         NOT NULL UNIQUE,
    username      VARCHAR(30)  NOT NULL UNIQUE,
    email         VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    created_at    TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at    TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TRIGGER update_users_updated_at
BEFORE UPDATE ON users
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();

INSERT INTO users (user_uuid, username, email, password_hash) VALUES
(
    '47f43b25-65c7-4993-a095-df55fc6bff61', 
    'yamada', 
    'yamada@example.com',    
    'AQAAAAIAAYagAAAAEDCrBvGA6zsEmsZOV3sQicBL2WCrRFyjmLZ7nw1TEOhSTwrlCbAy79r0mcOAjrw/LQ=='
),
(
    '35a30556-0efd-4c2e-871f-d9e8494b5f0f', 
    'tanaka', 
    'tanaka@example.com', 
    'AQAAAAIAAYagAAAAEJRelamSVCZALwErdKxzfMOYELRciEg2rVkFqo25+ankQSMAnqWzK9ghI9ml2d7nkw=='
);