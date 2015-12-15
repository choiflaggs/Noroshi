
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `shop`
  ADD COLUMN `PaymentPossessionCategory` tinyint unsigned NOT NULL AFTER `MerchandiseNum`,
  ADD COLUMN `PaymentPossessionID` int unsigned NOT NULL AFTER `PaymentPossessionCategory`,
  ADD COLUMN `ManualUpdatePossessionCategory` tinyint unsigned NOT NULL AFTER `PaymentPossessionID`,
  ADD COLUMN `ManualUpdatePossessionID` int unsigned NOT NULL AFTER `ManualUpdatePossessionCategory`,
  ADD COLUMN `ManualUpdatePossessionNum` int unsigned NOT NULL AFTER `ManualUpdatePossessionID`,
  ADD COLUMN `DailyScheduledUpdateNum` tinyint unsigned NOT NULL AFTER `ManualUpdatePossessionNum`,
  ADD COLUMN `RelatedGameContentID` int unsigned NOT NULL AFTER `DailyScheduledUpdateNum`,
  ADD COLUMN `AppearPlayerLevel` smallint unsigned NOT NULL AFTER `RelatedGameContentID`,
  ADD COLUMN `AppearRatio` float unsigned NOT NULL AFTER `AppearPlayerLevel`,
  ADD COLUMN `AppearMinute` smallint unsigned NOT NULL AFTER `AppearRatio`,
  ADD COLUMN `ResidentVipLevel` smallint unsigned NOT NULL AFTER `AppearMinute`;

DROP TABLE `shop_merchandise`;
CREATE TABLE `shop_merchandise` (
  `ID` int unsigned NOT NULL,
  `ShopID` int unsigned NOT NULL,
  `MinPlayerLevel` smallint unsigned NOT NULL,
  `MaxPlayerLevel` smallint unsigned NOT NULL,
  `MerchandisePossessionCategory` tinyint unsigned NOT NULL,
  `MerchandisePossessionID` int unsigned NOT NULL,
  `MerchandisePossessionNum` int unsigned NOT NULL,
  `PaymentPossessionCategory` tinyint unsigned NOT NULL,
  `PaymentPossessionID` int unsigned NOT NULL,
  `PaymentPossessionNum` int unsigned NOT NULL,
  `Weight` int unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `i1` (`ShopID`,`MaxPlayerLevel`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `shop`
  DROP COLUMN `PaymentPossessionCategory`,
  DROP COLUMN `PaymentPossessionID`,
  DROP COLUMN `ManualUpdatePossessionCategory`,
  DROP COLUMN `ManualUpdatePossessionID`,
  DROP COLUMN `ManualUpdatePossessionNum`,
  DROP COLUMN `DailyScheduledUpdateNum`,
  DROP COLUMN `RelatedGameContentID`,
  DROP COLUMN `AppearPlayerLevel`,
  DROP COLUMN `AppearRatio`,
  DROP COLUMN `AppearMinute`,
  DROP COLUMN `ResidentVipLevel`;


DROP TABLE `shop_merchandise`;
CREATE TABLE `shop_merchandise` (
  `ID` int(10) unsigned NOT NULL,
  `ShopID` int(10) unsigned NOT NULL,
  `MaxPlayerLevel` smallint(5) unsigned NOT NULL,
  `MinPlayerLevel` smallint(5) unsigned NOT NULL,
  `MerchandisePossessionCategory` tinyint(3) unsigned NOT NULL,
  `MerchandisePossessionID` int(10) unsigned NOT NULL,
  `MerchandisePossessionNum` int(10) unsigned NOT NULL,
  `PaymentPossessionCategory` tinyint(3) unsigned NOT NULL,
  `PaymentPossessionID` int(10) unsigned NOT NULL,
  `PaymentPossessionNum` int(10) unsigned NOT NULL,
  `Weight` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `i1` (`ShopID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

