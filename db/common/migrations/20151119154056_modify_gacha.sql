
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

DELETE FROM `gacha_entry_point`;
ALTER TABLE `gacha_entry_point`
  ADD COLUMN `TextKey` varchar(32) NOT NULL AFTER `ID`,
  ADD COLUMN `GameContentID` int unsigned NOT NULL AFTER `TextKey`,
  ADD COLUMN `OpenedAt` int unsigned NOT NULL AFTER `GameContentID`,
  ADD COLUMN `ClosedAt` int unsigned NOT NULL AFTER `OpenedAt`,
  ADD COLUMN `MaxTotalLotNum` tinyint unsigned NOT NULL AFTER `LotNum`,
  ADD COLUMN `MaxDailyFreeLotNum` tinyint unsigned NOT NULL AFTER `MaxTotalLotNum`,
  ADD COLUMN `FreeLotCoolTimeMinute` smallint unsigned NOT NULL AFTER `MaxDailyFreeLotNum`,
  ADD COLUMN `GuaranteedPossessionCategory` tinyint unsigned NOT NULL AFTER `PaymentPossessionNum`,
  ADD UNIQUE KEY `i1` (`TextKey`);

CREATE TABLE `gacha_guaranteed_lot` (
  `GachaID` int unsigned NOT NULL,
  `HitNum` int unsigned NOT NULL,
  `MissLotNum` int unsigned NOT NULL,
  `GuaranteedPossessionCategory` tinyint unsigned NOT NULL,
  PRIMARY KEY (`GachaID`,`HitNum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `gacha_entry_point`
  DROP COLUMN `TextKey`,
  DROP COLUMN `GameContentID`,
  DROP COLUMN `OpenedAt`,
  DROP COLUMN `ClosedAt`,
  DROP COLUMN `MaxTotalLotNum`,
  DROP COLUMN `MaxDailyFreeLotNum`,
  DROP COLUMN `FreeLotCoolTimeMinute`,
  DROP COLUMN `GuaranteedPossessionCategory`,
  DROP KEY `i1`;

DROP TABLE `gacha_guaranteed_lot`;

