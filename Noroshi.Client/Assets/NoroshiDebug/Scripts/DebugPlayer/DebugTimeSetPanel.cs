using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UniRx;
using Noroshi.UI;
using NoroshiDebug.TimeDebug;

namespace Noroshi.NoroshiDebug {
    public class DebugTimeSetPanel : MonoBehaviour {
        [SerializeField] GameObject timeSetWrapper;
        [SerializeField] Text txtCrntYear;
        [SerializeField] Text txtCrntMonth;
        [SerializeField] Text txtCrntDay;
        [SerializeField] Text txtCrntHour;
        [SerializeField] Text txtCrntMinute;
        [SerializeField] Text txtCrntSecond;
        [SerializeField] Text txtCrntDayOfWeek;
        [SerializeField] Text txtSettingYear;
        [SerializeField] Text txtSettingMonth;
        [SerializeField] Text txtSettingDay;
        [SerializeField] Text txtSettingHour;
        [SerializeField] Text txtSettingMinute;
        [SerializeField] Text txtSettingSecond;
        [SerializeField] Text txtSettingDayOfWeek;
        [SerializeField] BtnCommon btnSettingCrntTime;
        [SerializeField] BtnCommon btnSettingTime;
        [SerializeField] BtnCommon btnYearPlus;
        [SerializeField] BtnCommon btnYearMinus;
        [SerializeField] BtnCommon btnMonthPlus;
        [SerializeField] BtnCommon btnMonthMinus;
        [SerializeField] BtnCommon btnDayPlus;
        [SerializeField] BtnCommon btnDayMinus;
        [SerializeField] BtnCommon btnHourPlus;
        [SerializeField] BtnCommon btnHourMinus;
        [SerializeField] BtnCommon btnMinutePlus;
        [SerializeField] BtnCommon btnMinuteMinus;
        [SerializeField] BtnCommon btnSecondPlus;
        [SerializeField] BtnCommon btnSecondMinus;
        [SerializeField] BtnCommon btnClose;

        public Subject<DateTime> OnSetDebugTime = new Subject<DateTime>();

        private int settingYear;
        private int settingMonth;
        private int settingDay;
        private int settingHour;
        private int settingMinute;
        private int settingSecond;
        private int settingDayOfWeek;

        private TimeDebugRepository _timeDebug = new TimeDebugRepository();

        private void Start() {
            btnYearPlus.OnClickedBtn.Subscribe(ChangeYear);
            btnYearMinus.OnClickedBtn.Subscribe(ChangeYear);
            btnMonthPlus.OnClickedBtn.Subscribe(ChangeMonth);
            btnMonthMinus.OnClickedBtn.Subscribe(ChangeMonth);
            btnDayPlus.OnClickedBtn.Subscribe(ChangeDay);
            btnDayMinus.OnClickedBtn.Subscribe(ChangeDay);
            btnHourPlus.OnClickedBtn.Subscribe(ChangeHour);
            btnHourMinus.OnClickedBtn.Subscribe(ChangeHour);
            btnMinutePlus.OnClickedBtn.Subscribe(ChangeMinute);
            btnMinuteMinus.OnClickedBtn.Subscribe(ChangeMinute);
            btnSecondPlus.OnClickedBtn.Subscribe(ChangeSecond);
            btnSecondMinus.OnClickedBtn.Subscribe(ChangeSecond);

            btnSettingCrntTime.OnClickedBtn.Subscribe(_ => {
                DisableSettingBtn();
                _timeDebug.ChangeTime(0, 0, 0, 0, 0, 0).Subscribe(timeDebugData => {
                    OnSetDebugTime.OnNext(timeDebugData.DebugServerTime);
                    CloseTimeSetPanel();
                });
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });

            btnSettingTime.OnClickedBtn.Subscribe(_ => {
                SendDebugTime();
                SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
            });
            
            btnClose.OnClickedBtn.Subscribe(_ => {
                CloseTimeSetPanel();
            });
        }

        private void SendDebugTime() {
            DisableSettingBtn();
            _timeDebug.Get().Do(timeDebugData => {
                var newYear = settingYear - timeDebugData.ServerTime.Year;
                var newMonth = settingMonth - timeDebugData.ServerTime.Month;
                var newDay = settingDay - timeDebugData.ServerTime.Day;
                var newHour = settingHour - timeDebugData.ServerTime.Hour;
                var newMinute = settingMinute - timeDebugData.ServerTime.Minute;
                var newSecond = settingSecond - timeDebugData.ServerTime.Second;
                _timeDebug.ChangeTime(newYear, newMonth, newDay, newHour, newMinute, newSecond).Subscribe(changeData => {
                    OnSetDebugTime.OnNext(changeData.DebugServerTime);
                    CloseTimeSetPanel();
                });
            }).Subscribe();
        }

        private void SetCrntTime(DateTime crntTime) {
            var hour = crntTime.Hour;
            string hourString = hour < 10 ? "0" + hour : hour.ToString();
            var minute = crntTime.Minute;
            string minuteString = minute < 10 ? "0" + minute : minute.ToString();
            var second = crntTime.Second;
            string secondString = second < 10 ? "0" + second : second.ToString();
            txtCrntYear.text = crntTime.Year.ToString();
            txtCrntMonth.text = crntTime.Month.ToString();
            txtCrntDay.text = crntTime.Day.ToString();
            txtCrntHour.text = hourString;
            txtCrntMinute.text = minuteString;
            txtCrntSecond.text = secondString;
            txtCrntDayOfWeek.text = crntTime.DayOfWeek.ToString();
        }

        private void SetSettingTime() {
            var settingDate = new DateTime(settingYear, settingMonth, settingDay);
            txtSettingYear.text = settingYear.ToString();
            txtSettingMonth.text = settingMonth.ToString();
            txtSettingDay.text = settingDay.ToString();
            txtSettingHour.text = settingHour < 10 ? "0" + settingHour : settingHour.ToString();
            txtSettingMinute.text = settingMinute < 10 ? "0" + settingMinute : settingMinute.ToString();
            txtSettingSecond.text = settingSecond < 10 ? "0" + settingSecond : settingSecond.ToString();
            txtSettingDayOfWeek.text = settingDate.DayOfWeek.ToString();
        }
        
        private void ChangeYear(int value) {
            settingYear += value;
            if(settingYear > 2099) {settingYear = 2099;}
            if(settingYear < 2000) {settingYear = 2000;}
            ChangeDay(0);
            SetSettingTime();
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }
        
        private void ChangeMonth(int value) {
            settingMonth += value;
            if(settingMonth > 12) {settingMonth = 1;}
            if(settingMonth < 1) {settingMonth = 12;}
            ChangeDay(0);
            SetSettingTime();
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }
        
        private void ChangeDay(int value) {
            settingDay += value;
            if(settingYear % 4 == 0 && settingMonth == 2) {
                if(settingDay > 29) {settingDay = 1;}
                if(settingDay < 1) {settingDay = 29;}
            } else if(settingMonth == 2) {
                if(settingDay > 28) {settingDay = 1;}
                if(settingDay < 1) {settingDay = 28;}
            } else if(settingMonth == 4 || settingMonth == 6 || settingMonth == 9 || settingMonth == 11) {
                if(settingDay > 30) {settingDay = 1;}
                if(settingDay < 1) {settingDay = 30;}
            } else {
                if(settingDay > 31) {settingDay = 1;}
                if(settingDay < 1) {settingDay = 31;}
            }
            SetSettingTime();
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }
        
        private void ChangeHour(int value) {
            settingHour += value;
            if(settingHour > 23) {settingHour = 0;}
            if(settingHour < 0) {settingHour = 23;}
            SetSettingTime();
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }
        
        private void ChangeMinute(int value) {
            settingMinute += value;
            if(settingMinute > 59) {settingMinute = 0;}
            if(settingMinute < 0) {settingMinute = 59;}
            SetSettingTime();
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }

        private void ChangeSecond(int value) {
            settingSecond += value;
            if(settingSecond > 59) {settingSecond = 0;}
            if(settingSecond < 0) {settingSecond = 59;}
            SetSettingTime();
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }

        private void EnableSettingBtn() {
            btnSettingCrntTime.SetEnable(true);
            btnSettingTime.SetEnable(true);
            TweenA.Add(btnSettingCrntTime.gameObject, 0.01f, 1);
            TweenA.Add(btnSettingTime.gameObject, 0.01f, 1);
        }

        private void DisableSettingBtn() {
            btnSettingCrntTime.SetEnable(false);
            btnSettingTime.SetEnable(false);
            TweenA.Add(btnSettingCrntTime.gameObject, 0.01f, 0.5f);
            TweenA.Add(btnSettingTime.gameObject, 0.01f, 0.5f);
        }
        
        private void CloseTimeSetPanel() {
            TweenA.Add(timeSetWrapper, 0.2f, 0).EaseOutCubic().Then(() => {
                gameObject.SetActive(false);
            });
            SoundController.Instance.PlaySE(SoundController.SEKeys.SELECT);
        }

        public void OpenTimeSetPanel() {
            gameObject.SetActive(true);
            _timeDebug.Get().Do(timeDebugData => {
                settingYear = timeDebugData.DebugServerTime.Year;
                settingMonth = timeDebugData.DebugServerTime.Month;
                settingDay = timeDebugData.DebugServerTime.Day;
                settingHour = timeDebugData.DebugServerTime.Hour;
                settingMinute = timeDebugData.DebugServerTime.Minute;
                settingSecond = timeDebugData.DebugServerTime.Second;
                SetCrntTime(timeDebugData.ServerTime);
                SetSettingTime();
                EnableSettingBtn();
                TweenA.Add(timeSetWrapper, 0.2f, 1).From(0).EaseInCubic();
            }).Subscribe();
        }
    }
}
