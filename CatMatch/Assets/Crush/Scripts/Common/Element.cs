
public enum EntityType
{
    None,// All trong team buff
    Beast,
    Gate
}

public enum TeamBuff
{
    None,
    Hp,
    EnemyHp,
    Def,
    EnemyDef,
    Atk,
    EnemyAtk,    
    MoveSpeed,
    EnemyMoveSpeed,
    CritFactor,
    EnemyCritFactor,
    FreezeDuration,
    EnemyFreezeDuration,
    AtkPerSec,
    EnemyAtkPerSec,
    ChanceSpawnFrenzy,/// cơ hội xx tất cả chỉ số
    EnemyChanceSpawnFrenzy,
    MakeFreezeExplode,// con nào có skill freeze thì khi hết time freeze sẽ nổ
    EnemyMakeFreezeExplode
}

public enum Element
{
    None,// All trong team buff
    Dark,
    Fire,
    Grass,
    Water,
    Light,
    All,
    Self
}

public enum BeastClass
{
    Samurai,
    Brawler,
    Knight,
    Archer,
    Barbarian,
    Ranger,
    Mage,
    Lancer,
    Rougue,
    Gunner,
    Support,
    None
}

public enum BeastId
{
    Atlantus,
    BlackBeard,
    BugonautArcher,
    Chaos,
    GrooVine,
    IceKnight,
    Kage,
    Magmus,
    MechaValken,
    Neko,
    RobinHood,
    Spike,
    Thorn,
    VulcanArcher,
    BlueFish,
    Valkyrie,
    Pirato,
    Akwa,
    Misty,
    Scud,
    MonkeyKing,
    LeafBlade,
    Furiosa,
    SiegFried,
    OneEye,
    FrostQueen,
    Onyx,
    Merlinus,
    Circle,
    DarkHunter,
    Vlad,
    Sorrow
}

public enum Rarity
{
    Rare,
    Common,
    Epic,
    Legend,
    Boss
}