
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `login_bonus_reward`
  DROP INDEX `i1`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `login_bonus_reward`
  ADD UNIQUE KEY `i1` (`LoginBonusID`,`Threshold`);
