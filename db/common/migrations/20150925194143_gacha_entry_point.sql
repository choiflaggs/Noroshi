
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `gacha_entry_point` (
  `ID` int unsigned NOT NULL,
  `GachaID` int unsigned NOT NULL,
  `LotNum` tinyint unsigned NOT NULL,
  `PaymentPossessionCategory` tinyint unsigned NOT NULL,
  `PaymentPossessionID` int unsigned NOT NULL,
  `PaymentPossessionNum` int unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

ALTER TABLE `gacha_possession_content` RENAME `gacha_content`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `gacha_entry_point`;
ALTER TABLE `gacha_content` RENAME `gacha_possession_content`;
