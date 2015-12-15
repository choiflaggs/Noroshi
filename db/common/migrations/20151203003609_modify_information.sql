
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `information`
  DROP COLUMN `BodyTextKey`,
  CHANGE COLUMN `TitleTextKey` `TextKey` varchar(32) NOT NULL;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `information`
  CHANGE COLUMN `TextKey` `TitleTextKey` varchar(32) NOT NULL,
  ADD COLUMN `BodyTextKey` varchar(32) NOT NULL AFTER `TitleTextKey`,
  ADD UNIQUE KEY `Body_UNIQUE` (`BodyTextKey`);
