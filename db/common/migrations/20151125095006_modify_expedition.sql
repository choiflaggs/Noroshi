
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `expedition` (
  `ID` int unsigned NOT NULL,
  `Level` tinyint unsigned NOT NULL,
  `AutomaticRecovery` tinyint unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

ALTER TABLE `expedition_stage`
  CHANGE COLUMN `Step` `Step` tinyint unsigned NOT NULL,
  CHANGE COLUMN `Money` `Gold` int unsigned NOT NULL,
  ADD COLUMN `ExpeditionPoint` int unsigned NOT NULL AFTER `Gold`,
  ADD COLUMN `GachaLotNum` tinyint unsigned NOT NULL AFTER `GachaID`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `expedition`;

ALTER TABLE `expedition_stage`
  CHANGE COLUMN `Step` `Step` tinyint NOT NULL,
  CHANGE COLUMN `Gold` `Money` int unsigned NOT NULL,
  DROP COLUMN `ExpeditionPoint`,
  DROP COLUMN `GachaLotNum`;

