
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `player_device_token`
  ADD UNIQUE KEY `Type_Token_UNIQUE` (`Type`,`Token`);

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `player_device_token`
  DROP KEY `Type_Token_UNIQUE`;

