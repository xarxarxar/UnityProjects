using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MergeBeast
{
    public class BookHeroPanel : MonoBehaviour
    {
        [SerializeField] private HeroConfig _config;
        [SerializeField] private BookHeroItem _prfHero;
        [SerializeField] private Transform _heroParent;

        [Header("Hero Info")]
        [SerializeField] private Image _imgBGHero;
        [SerializeField] private Image _imgMale;
        [SerializeField] private Image _imgHero;
        [SerializeField] private Text _txtRarity;
        [SerializeField] private List<GameObject> _listStar;
        [SerializeField] private Sprite _spriteBGCommon;
        [SerializeField] private Sprite _spriteBGEpic;
        [SerializeField] private Sprite _spriteBGRare;
        [SerializeField] private Sprite _spriteMale;
        [SerializeField] private Sprite _sprFemale;

        [Header("Hero Stats")]
        [SerializeField] private Text _txtHeroName;
        [SerializeField] private Text _txtDps;
        [SerializeField] private Text _txtDpsBonus;
        [SerializeField] private Text _txtLuck;
        [SerializeField] private Text _txtLuckBonus;
        [SerializeField] private Image _imgSkill;

        private List<BookHeroItem> _listHeroItem;
        // Use this for initialization
        void Start()
        {
            _listHeroItem = new List<BookHeroItem>();

            foreach(var hr in _config.Heroes)
            {
                BookHeroItem hero = Instantiate(_prfHero, _heroParent);
                hero.ImgAvatar.sprite = hr.Value.Avatar;
                HeroData data = hr.Value;
                hero.BtnHero?.onClick.AddListener(() => this.OnClickPickHero(data));
            }

        }
        
        private void OnClickPickHero(HeroData data)
        {

            _imgMale.sprite = data.Sex == EnumDefine.SEX.MALE ? _spriteMale : _sprFemale;
            _imgHero.sprite = data.Character;
            _imgHero.SetNativeSize();
            _txtHeroName.text = data.Name;
            _txtDps.text = $"DPS{data.MaxDps}%";
            _txtLuck.text = $"幸运{data.MaxLuck}%";
            _imgSkill.sprite = data.SkillIcon;

            switch (data.Rarity)
            {
                case EnumDefine.RARITY.COMMON:
                    _imgBGHero.sprite = _spriteBGCommon;
                    _txtRarity.text = "稀有度：<color=#44F427FF>普通</color>";
                    break;
                case EnumDefine.RARITY.EPIC:
                    _imgBGHero.sprite = _spriteBGEpic;
                    _txtRarity.text = "稀有度：<color=#9800F1FF>史诗</color>";
                    break;
                case EnumDefine.RARITY.RARE:
                    _imgBGHero.sprite = _spriteBGRare;
                    _txtRarity.text = "稀有度：<color=#5574CFFF>稀有</color>";
                    break;
            }
        }
    }
}
