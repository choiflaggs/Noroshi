
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `shop`
  CHANGE COLUMN `AppearRatio` `AppearProbability` float unsigned NOT NULL,
  DROP COLUMN `MerchandiseNum`;

ALTER TABLE `shop_merchandise`
  CHANGE COLUMN `ShopID` `MerchandiseGroupID` int unsigned NOT NULL;

CREATE TABLE `shop_display` (
  `ID` int unsigned NOT NULL,
  `ShopID` int unsigned NOT NULL,
  `No` tinyint unsigned NOT NULL,
  `MerchandiseGroupID` int unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`ShopID`,`No`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `shop`
  CHANGE COLUMN `AppearProbability` `AppearRatio` float unsigned NOT NULL,
  ADD COLUMN `MerchandiseNum` tinyint unsigned NOT NULL AFTER `TextKey`;

ALTER TABLE `shop_merchandise`
  CHANGE COLUMN `MerchandiseGroupID` `ShopID` int unsigned NOT NULL;

DROP TABLE `shop_display`;

