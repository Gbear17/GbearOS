// 
A B;C D;E F;G H;I J;K L;IMyBroadcastListener M;N O=new N();P Q=new P();R S=new R();T U=new T();V W=new V();X Y=new X();
private readonly StringBuilder Z=new StringBuilder(768);List<IMyTerminalBlock>a=new List<IMyTerminalBlock>();private readonly
List<IMyCargoContainer>b=new List<IMyCargoContainer>();private readonly List<IMyRefinery>c=new List<IMyRefinery>();private
readonly List<IMyGasTank>d=new List<IMyGasTank>();private readonly List<IMyGasGenerator>e=new List<IMyGasGenerator>();private
readonly List<IMyBatteryBlock>f=new List<IMyBatteryBlock>();bool g;int h=0;DateTime i=DateTime.MinValue;string j=string.Empty;
private const float k=95f;private readonly List<MyProductionItem>l=new List<MyProductionItem>();public
 Program
(){Runtime.UpdateFrequency=UpdateFrequency.Update10;D=new C(Me);B=D.m();if(string.IsNullOrEmpty(B.n)){const string o="GbearOS WARNING: [Network] SharedKey is empty — IGC DTOs use SenderEnvelope with no shared secret (unauthenticated). Set SharedKey on PB1 and PB2 CustomData."
;Echo(o);}E.p=this;E.q=B;F=new E();H=new G();J=new I();L=new K();M=IGC.RegisterBroadcastListener(r.s);M.
SetMessageCallback("PB2_MSG");}public void
 Save
(){}public void
 Main
(string t,UpdateType u){if(v(t)){return;}try{if((u&UpdateType.Update10)!=0){w();string y=x();Echo(y);IGC.
SendBroadcastMessage(r.z,y);}if((u&UpdateType.IGC)!=0){ª();}}catch(Exception µ){if(º(µ)){i=DateTime.UtcNow.AddSeconds(60);j=µ.Message??
string.Empty;return;}string À="PB1 ERROR:\n"+µ.ToString();Echo(À);IGC.SendBroadcastMessage(r.z,À);}}bool v(string t){if(t==
null){}return DateTime.UtcNow<i;}private static bool º(Exception µ){for(Exception Á=µ;Á!=null;Á=Á.InnerException){string Â=Á
.GetType().Name;if(Â.IndexOf("GracefulShutDown",StringComparison.OrdinalIgnoreCase)>=0){return true;}}return false;}void
w(){if(h%5==0&&F.Ã()){a.Clear();GridTerminalSystem.GetBlocks(a);E.Ä(a,b,c,d,e,f);}int Å=h%5;bool Æ=false;switch(Å){case 0
:Æ=F.Ç(a,b,c,ref O,ref Y);if(Æ){D.È(B,F.É);}break;case 1:Æ=H.Ê(a,c,ref Q);if(Æ){g=Ë(a);}break;case 2:Æ=J.Ì(a,b,e,d,ref S)
;break;case 3:Æ=L.Í(a,f,ref U);break;case 4:Æ=true;break;}if(Æ){h++;}Î();Ï(W);if(B.Ð&&!string.IsNullOrEmpty(B.n)){Ñ(r.Ò,O
);Ñ(r.Ó,Q);Ñ(r.Ô,S);Ñ(r.Õ,U);Ñ(r.Ö,Y);}Ñ(r.Ø,W);}string x(){Z.Clear();Z.Append("=== ");Z.Append(B.Ù);Z.AppendLine(
" ORCHESTRATOR ===");Z.Append("Instructions: ");Z.Append(Runtime.CurrentInstructionCount);Z.Append(" / ");Z.Append(Runtime.
MaxInstructionCount);Z.AppendLine();Z.Append("Last Run: ");Z.Append(Runtime.LastRunTimeMs.ToString("F4"));Z.AppendLine(" ms");Z.Append(
"CARGO: ");if(O.Ú>=k){Z.AppendLine("FULL");}else{Z.AppendLine("Nominal");}Z.Append("POWER: ");if(U.Û){Z.AppendLine("LOW CHARGE");
}else{Z.AppendLine("Nominal");}Z.Append(H.Ü(B!=null&&B.Ý));return Z.ToString();}void ª(){while(M.HasPendingMessage){var Þ
=M.AcceptMessage();}}void Ñ<ß>(string à,ß á){if(string.IsNullOrEmpty(B.n)){Echo("NET BLOCKED: SharedKey missing.");return
;}string ä=â.ã(á);string ç=å.æ(B.Ù,ä,B.n);IGC.SendBroadcastMessage(à,ç);}void Î(){V o=W;o.Û=U.Û;o.è=S.è;o.é=O.ê>0.0001f&&
O.Ú>=k;string[]ë=Q.ë;o.ì=ë==null||ë.Length==0;o.í=î(Q);o.ï=g;}private static bool î(P ð){bool[]ñ=ð.ñ;bool[]ò=ð.ò;if(ñ==
null||ò==null){return false;}int Â=ñ.Length<ò.Length?ñ.Length:ò.Length;for(int ó=0;ó<Â;ó++){if(ñ[ó]&&!ò[ó]){return true;}}
return false;}bool Ë(List<IMyTerminalBlock>ô){if(ô==null){return false;}string ö=B.õ;for(int ó=0;ó<ô.Count;ó++){
IMyTerminalBlock ø=ô[ó];var ù=ø as IMyAssembler;if(ù==null||!ù.IsSameConstructAs(Me)){continue;}if(!string.IsNullOrEmpty(ö)){string ú=ù.
CustomName;if(û.ü(ú,ö)){continue;}}var ý=ù as IMyProductionBlock;if(ý==null){continue;}l.Clear();ý.GetQueue(l);if(l.Count>0&&!ý.
IsProducing){return true;}}return false;}void Ï(V á){á.þ=false;if(á.Û){á.ÿ=0;á.Ā="LOW POWER";return;}if(á.é){á.ÿ=1;á.Ā="CARGO FULL"
;return;}if(á.è){á.ÿ=2;á.Ā="LOW ICE";return;}if(á.í){á.ÿ=3;á.Ā="REFINERY STALLED";return;}if(á.ï){á.ÿ=4;á.Ā=
"ASSEMBLER STALLED";return;}if(á.ì){á.ÿ=5;á.Ā="NO REFINERIES";return;}á.ÿ=-1;á.Ā="NOMINAL";á.þ=true;}
}
public class A{public Dictionary<string,double>ā=new Dictionary<string,double>(StringComparer.OrdinalIgnoreCase);public
double Ă=50000,ă=10000,Ą=15000,ą=5000,Ć=200000,ć=50000,Ĉ=200,ĉ=50,Ċ=0.25,ċ=0.80,Č=0.20,č=0.70,Ď=0.40,ď=0.90,Đ=0.35,đ=0.85,Ē=
0.25,ē=0.80,Ĕ=0.05,ĕ=0.05;public bool Ė=false,Ý=true,ė=true,Ę=true,ę=true,Ě=true,ě=true,Ĝ=false,Ð=true;public string ĝ=
"[Irrigator]",õ="[Manual]",Ù="CMD-DEFAULT",n="";}public class C{private readonly IMyProgrammableBlock Ğ;private readonly VRage.Game.
ModAPI.Ingame.Utilities.MyIni ğ=new VRage.Game.ModAPI.Ingame.Utilities.MyIni();private const string Ġ="IngotTargets",ġ=
"IceTargets",Ģ="ReactorTargets",ģ="BatteryThresholds",Ĥ="RefinerySettings",ĥ="BlockTags",Ħ="DisplayFilters",ħ="Debug",Ĩ="Network",ĩ=
"Documentation";public C(IMyProgrammableBlock Ī){Ğ=Ī;}public A m(){ī();var Ĭ=new A();ĭ(Ĭ);Į(Ĭ);į(Ĭ);İ(Ĭ);Ğ.CustomData=ğ.ToString();
return Ĭ;}public void È(A ı,List<string>Ĳ){if(ı==null||Ĳ==null){return;}ī();ĭ(ı);Į(ı);bool ĳ=false;if(ı.ā!=null){for(int ó=0;ó
<Ĳ.Count;ó++){string Ĵ=Ĳ[ó];if(string.IsNullOrEmpty(Ĵ)||E.ĵ(Ĵ)||ı.ā.ContainsKey(Ĵ)||ı.ā.ContainsKey("Ingot/"+Ĵ)){continue
;}if(!ğ.ContainsKey(Ġ,Ĵ)){ğ.Set(Ġ,Ĵ,500);}ı.ā[Ĵ]=500;ĳ=true;}}if(!ĳ){return;}į(ı);İ(ı);Ğ.CustomData=ğ.ToString();}private
static string ļ(string Ķ,string ķ){if(string.IsNullOrEmpty(Ķ))return ķ;int ĸ=Ķ.IndexOf('-');string Ĺ=ĸ<0?Ķ:Ķ.Substring(0,ĸ);
string ĺ="";for(int ó=0;ó<Ĺ.Length&&ĺ.Length<3;ó++){char Ļ=Ĺ[ó];if(char.IsLetterOrDigit(Ļ))ĺ+=char.ToUpperInvariant(Ļ);}return
ĺ.Length>0?ĺ:ķ;}string Ł(string Ľ,string ľ){string Ŀ=Ğ.EntityId.ToString("X");Ŀ=Ŀ.Substring(Math.Max(0,Ŀ.Length-4));
string ŀ=ļ(Ľ,ľ);return ŀ+"-"+Ŀ;}void ī(){VRage.Game.ModAPI.Ingame.Utilities.MyIniParseResult ł;if(!ğ.TryParse(Ğ.CustomData??""
,out ł)){ğ.Clear();}}void ĭ(A Ĭ){Ĭ.ā.Clear();var Ń=new List<VRage.Game.ModAPI.Ingame.Utilities.MyIniKey>();ğ.GetKeys(Ġ,Ń)
;for(int ó=0;ó<Ń.Count;ó++){string ń=Ń[ó].Name;double Ņ=ğ.Get(Ġ,ń).ToDouble(0);if(Ņ<0){Ņ=0;}Ĭ.ā[ń]=Ņ;}Ĭ.Ă=ņ(ğ.Get(ġ,
"GeneratorLargeTargetIce").ToDouble(50000));Ĭ.ă=ņ(ğ.Get(ġ,"GeneratorSmallTargetIce").ToDouble(10000));Ĭ.Ą=ņ(ğ.Get(ġ,"IrrigationLargeTargetIce").
ToDouble(15000));Ĭ.ą=ņ(ğ.Get(ġ,"IrrigationSmallTargetIce").ToDouble(5000));Ĭ.Ć=ņ(ğ.Get(ġ,"CargoReserveIce").ToDouble(200000));Ĭ.
ć=ņ(ğ.Get(ġ,"MinimumCargoIce").ToDouble(50000));Ĭ.Ĉ=ņ(ğ.Get(Ģ,"ReactorLargeUraniumTarget").ToDouble(200));Ĭ.ĉ=ņ(ğ.Get(Ģ,
"ReactorSmallUraniumTarget").ToDouble(50));Ĭ.Ċ=Ň(ğ.Get(ģ,"ReactorLargeLower").ToDouble(0.25));Ĭ.ċ=Ň(ğ.Get(ģ,"ReactorLargeUpper").ToDouble(0.80));Ĭ.
Č=Ň(ğ.Get(ģ,"ReactorSmallLower").ToDouble(0.20));Ĭ.č=Ň(ğ.Get(ģ,"ReactorSmallUpper").ToDouble(0.70));Ĭ.Ď=Ň(ğ.Get(ģ,
"EngineLargeLower").ToDouble(0.40));Ĭ.ď=Ň(ğ.Get(ģ,"EngineLargeUpper").ToDouble(0.90));Ĭ.Đ=Ň(ğ.Get(ģ,"EngineSmallLower").ToDouble(0.35));Ĭ.
đ=Ň(ğ.Get(ģ,"EngineSmallUpper").ToDouble(0.85));Ĭ.Ē=Ň(ğ.Get(ģ,"BatteryChargeTarget").ToDouble(0.25));Ĭ.ē=Ň(ğ.Get(ģ,
"BatteryDischargeTarget").ToDouble(0.80));Ĭ.Ė=ğ.Get(ģ,"EnablePowerAutomation").ToBoolean(false);Ĭ.Ĕ=ņ(ğ.Get(ģ,"SolarMinimumOutput").ToDouble(
0.05));Ĭ.Ý=ğ.Get(Ĥ,"EnableRefineryBalancing").ToBoolean(true);Ĭ.ĕ=ņ(ğ.Get(Ĥ,"RefineryHysteresis").ToDouble(0.05));Ĭ.ĝ=ğ.Get(
ĥ,"IrrigationTag").ToString("[Irrigator]");Ĭ.õ=ğ.Get(ĥ,"ManualTag").ToString("[Manual]");if(Ĭ.ĝ!=null){Ĭ.ĝ=Ĭ.ĝ.Trim();}if
(Ĭ.õ!=null){Ĭ.õ=Ĭ.õ.Trim();}Ĭ.ė=ğ.Get(Ħ,"ShowOres").ToBoolean(true);Ĭ.Ę=ğ.Get(Ħ,"ShowIngots").ToBoolean(true);Ĭ.ę=ğ.Get(Ħ
,"ShowComponents").ToBoolean(true);Ĭ.Ě=ğ.Get(Ħ,"ShowAmmo").ToBoolean(true);Ĭ.ě=ğ.Get(Ħ,"ShowDynamicItems").ToBoolean(true
);Ĭ.Ĝ=ğ.Get(ħ,"EnableDebug").ToBoolean(false);string ň=ğ.Get(Ĩ,"PBID").ToString("");if(ň!=null)ň=ň.Trim();Ĭ.Ù=Ł(ň??"",
"CMD");Ĭ.n=ğ.Get(Ĩ,"SharedKey").ToString("");Ĭ.Ð=ğ.Get(Ĩ,"EnableNetwork").ToBoolean(true);if(Ĭ.n!=null){Ĭ.n=Ĭ.n.Trim();}}void
į(A ı){var ŉ=new List<string>(ı.ā.Keys);for(int Ŋ=0;Ŋ<ŉ.Count;Ŋ++){string ŋ=ŉ[Ŋ];if(ı.ā[ŋ]<0){ı.ā[ŋ]=0;}}ı.Ă=ņ(ı.Ă);ı.ă=ņ
(ı.ă);ı.Ą=ņ(ı.Ą);ı.ą=ņ(ı.ą);ı.Ć=ņ(ı.Ć);ı.ć=ņ(ı.ć);ı.Ĉ=ņ(ı.Ĉ);ı.ĉ=ņ(ı.ĉ);ı.Ċ=Ň(ı.Ċ);ı.ċ=Ň(ı.ċ);ı.Č=Ň(ı.Č);ı.č=Ň(ı.č);ı.Ď=Ň
(ı.Ď);ı.ď=Ň(ı.ď);ı.Đ=Ň(ı.Đ);ı.đ=Ň(ı.đ);ı.Ē=Ň(ı.Ē);ı.ē=Ň(ı.ē);ı.Ĕ=ņ(ı.Ĕ);ı.ĕ=ņ(ı.ĕ);if(ı.n!=null){ı.n=ı.n.Trim();}ı.Ù=Ł(ı.
Ù==null?"":ı.Ù.Trim(),"CMD");}void İ(A Ĭ){ğ.Clear();ğ.Set(ĩ,"ConfigurationManual","docs/configuration.md");ğ.SetComment(ĩ
,"ConfigurationManual","Full Custom Data reference for PB1/PB2 — open in the GbearOS repository (see also docs/architecture/user_config_system.md)."
);ğ.Set(Ĩ,"EnableNetwork",Ĭ.Ð);ğ.SetComment(Ĩ,"EnableNetwork",
"See docs/configuration.md — set false for offline mode (no IGC DTO send).");ğ.Set(Ĩ,"PBID",Ĭ.Ù??"CMD-0000");ğ.SetComment(Ĩ,"PBID","Format: ABC-XXXX. You may change the 3-letter prefix. The 4-character suffix is locked to this block's ID and will auto-reset if changed."
);ğ.Set(Ĩ,"SharedKey",Ĭ.n??"");ğ.SetComment(Ĩ,"SharedKey","MAC secret; must match PB2.");ğ.Set(ġ,
"GeneratorLargeTargetIce",Ĭ.Ă);ğ.Set(ġ,"GeneratorSmallTargetIce",Ĭ.ă);ğ.Set(ġ,"IrrigationLargeTargetIce",Ĭ.Ą);ğ.Set(ġ,"IrrigationSmallTargetIce",
Ĭ.ą);ğ.Set(ġ,"CargoReserveIce",Ĭ.Ć);ğ.SetComment(ġ,"CargoReserveIce","Triggers LOW ICE warning.");ğ.Set(ġ,
"MinimumCargoIce",Ĭ.ć);ğ.SetComment(ġ,"MinimumCargoIce","Cargo ice above: to gen/irr.");ğ.Set(Ģ,"ReactorLargeUraniumTarget",Ĭ.Ĉ);ğ.Set(Ģ,
"ReactorSmallUraniumTarget",Ĭ.ĉ);ğ.Set(ģ,"ReactorLargeLower",Ĭ.Ċ);ğ.Set(ģ,"ReactorLargeUpper",Ĭ.ċ);ğ.Set(ģ,"ReactorSmallLower",Ĭ.Č);ğ.Set(ģ,
"ReactorSmallUpper",Ĭ.č);ğ.Set(ģ,"EngineLargeLower",Ĭ.Ď);ğ.Set(ģ,"EngineLargeUpper",Ĭ.ď);ğ.Set(ģ,"EngineSmallLower",Ĭ.Đ);ğ.Set(ģ,
"EngineSmallUpper",Ĭ.đ);ğ.Set(ģ,"BatteryChargeTarget",Ĭ.Ē);ğ.SetComment(ģ,"BatteryChargeTarget","Recharge below this fraction.");ğ.Set(ģ,
"BatteryDischargeTarget",Ĭ.ē);ğ.SetComment(ģ,"BatteryDischargeTarget","Auto above this fraction.");ğ.Set(ģ,"EnablePowerAutomation",Ĭ.Ė);ğ.
SetComment(ģ,"EnablePowerAutomation","Solar-driven reactor/engine toggle.");ğ.Set(ģ,"SolarMinimumOutput",Ĭ.Ĕ);ğ.SetComment(ģ,
"SolarMinimumOutput","Backup if solar below this MW.");ğ.Set(Ĥ,"EnableRefineryBalancing",Ĭ.Ý);ğ.SetComment(Ĥ,"EnableRefineryBalancing",
"Script queues; off = vanilla.");ğ.Set(Ĥ,"RefineryHysteresis",Ĭ.ĕ);ğ.SetComment(Ĥ,"RefineryHysteresis","Top-ore switch hysteresis.");ğ.Set(ĥ,
"IrrigationTag",Ĭ.ĝ??"[Irrigator]");ğ.SetComment(ĥ,"IrrigationTag","O2/H2 farm ice supply tag.");ğ.Set(ĥ,"ManualTag",Ĭ.õ??"[Manual]");ğ
.SetComment(ĥ,"ManualTag","Ignore tagged blocks.");ğ.Set(Ħ,"ShowOres",Ĭ.ė);ğ.Set(Ħ,"ShowIngots",Ĭ.Ę);ğ.Set(Ħ,
"ShowComponents",Ĭ.ę);ğ.Set(Ħ,"ShowAmmo",Ĭ.Ě);ğ.Set(Ħ,"ShowDynamicItems",Ĭ.ě);ğ.Set(ħ,"EnableDebug",Ĭ.Ĝ);foreach(var Ō in Ĭ.ā){ğ.Set(Ġ,Ō
.Key,Ō.Value);}}private static void Į(A ı){if(ı==null||ı.ā==null||ı.ā.Count>0){return;}ō(ı.ā);}private static void ő(
Dictionary<string,double>Ŏ,string ŏ,double Ő){if(!Ŏ.ContainsKey(ŏ)){Ŏ[ŏ]=Ő;}}private static void ō(Dictionary<string,double>Ŏ){ő(Ŏ
,"Iron",125000);ő(Ŏ,"Nickel",25800);ő(Ŏ,"Silicon",17500);ő(Ŏ,"Cobalt",14800);ő(Ŏ,"Silver",6100);ő(Ŏ,"Gold",9000);ő(Ŏ,
"Magnesium",15000);ő(Ŏ,"Platinum",4500);ő(Ŏ,"Uranium",2600);ő(Ŏ,"Gravel",22500);}private static double ņ(double Ņ){return Ņ<0?0:Ņ;}
private static double Ň(double Ņ){if(Ņ<0){return 0;}return Ņ>1?1:Ņ;}}public class I{private static readonly MyItemType Œ=
MyItemType.MakeOre("Ice");private readonly List<IMyGasGenerator>œ=new List<IMyGasGenerator>();private readonly List<
IMyTerminalBlock>Ŕ=new List<IMyTerminalBlock>();private readonly List<IMyCargoContainer>ŕ=new List<IMyCargoContainer>();double Ŗ,ŗ,Ř;
private const int ř=38000;int Ś;byte ś=4;bool Ŝ;public bool Ì(List<IMyTerminalBlock>ŝ,List<IMyCargoContainer>Ş,List<
IMyGasGenerator>ş,List<IMyGasTank>Š,ref R á){if(!š()){ś=4;Ţ(ref á);return true;}if(ś==4){œ.Clear();Ŕ.Clear();ŕ.Clear();Ŝ=false;ś=0;Ś=0;
}if(ś==0){if(!ţ(ŝ,Ş,ş)){return false;}ś=1;return false;}if(ś==1){Ť(á,ť());ś=2;return false;}Ŧ();ś=4;return true;}private
static void Ţ(ref R ŧ){ŧ.Ũ=0f;ŧ.ũ=0f;ŧ.Ū=0f;ŧ.ū=0f;ŧ.Ŭ=0f;ŧ.ŭ=0f;ŧ.Ů=0f;ŧ.ů=0f;ŧ.Ű=0;ŧ.ű=0;ŧ.è=false;}private static void Ť(R
Ų,R ų){Ų.Ũ=ų.Ũ;Ų.ũ=ų.ũ;Ų.Ū=ų.Ū;Ų.ū=ų.ū;Ų.Ŭ=ų.Ŭ;Ų.ŭ=ų.ŭ;Ų.Ů=ų.Ů;Ų.ů=ų.ů;Ų.Ű=ų.Ű;Ų.ű=ų.ű;Ų.è=ų.è;}bool ţ(List<
IMyTerminalBlock>ŝ,List<IMyCargoContainer>Ş,List<IMyGasGenerator>ş){if(ŝ==null){return true;}if(!Ŝ){if(Ş!=null){for(int Ŵ=0;Ŵ<Ş.Count;Ŵ
++){var Ĭ=Ş[Ŵ];if(Ĭ!=null){ŕ.Add(Ĭ);}}}if(ş!=null){for(int ŵ=0;ŵ<ş.Count;ŵ++){var Ŷ=ş[ŵ];if(Ŷ!=null){œ.Add(Ŷ);}}}Ŝ=true;}
for(int ó=Ś;ó<ŝ.Count;ó++){if(E.p.Runtime.CurrentInstructionCount>ř){Ś=ó;return false;}var ø=ŝ[ó];if(!ŷ(ø)){continue;}if(ø
is IMyGasGenerator){continue;}if(ø is IMyCargoContainer){continue;}if(Ÿ(ø)){Ŕ.Add(ø);}}return true;}bool ŷ(
IMyTerminalBlock ø){if(!š()||ø==null){return false;}if(!ø.IsSameConstructAs(E.p.Me)){return false;}if(Ź(ø)){return false;}if(ø is
IMyGasGenerator){return true;}if(ø is IMyCargoContainer){return true;}return Ÿ(ø);}void Ž(){Ŗ=ź();ŗ=Ż();Ř=ż();}void Ŧ(){var ž=E.q;if(ž
==null){return;}double ſ=ž.ć;if(ſ<0){ſ=0;}const double ƀ=0.001;for(int Ɓ=0;Ɓ<3;Ɓ++){double Ƃ=ż();double ƃ=Ƃ-ſ;if(ƃ<=ƀ){
break;}if(!Ƅ(ž,ƃ)){break;}}}bool Ƅ(A ž,double ƅ){const double ƀ=0.001;IMyInventoryOwner Ɔ=null;double Ƈ=ƀ;for(int Ŷ=0;Ŷ<œ.
Count;Ŷ++){var ƈ=œ[Ŷ];var Ɖ=ƈ as IMyInventoryOwner;if(Ɖ==null){continue;}bool Ɗ=ƈ.CubeGrid.GridSizeEnum==MyCubeSize.Large;
double Ƌ=Ɗ?ž.Ă:ž.ă;double ƍ=ƌ(Ɖ);double Ǝ=Ƌ-ƍ;if(Ǝ<=ƀ){continue;}if(Ǝ>Ƈ){Ƈ=Ǝ;Ɔ=Ɖ;}}for(int ł=0;ł<Ŕ.Count;ł++){var Ə=Ŕ[ł];bool
Ɛ=Ə.CubeGrid.GridSizeEnum==MyCubeSize.Large;double Ƌ=Ɛ?ž.Ą:ž.ą;var Ƒ=Ə as IMyInventoryOwner;if(Ƒ==null){continue;}double
ƍ=ƌ(Ƒ);double Ǝ=Ƌ-ƍ;if(Ǝ<=ƀ){continue;}if(Ǝ>Ƈ){Ƈ=Ǝ;Ɔ=Ƒ;}}if(Ɔ==null){return false;}double ƒ=ƅ;if(Ƈ<ƒ){ƒ=Ƈ;}for(int Ɠ=0;Ɠ<
Ɔ.InventoryCount;Ɠ++){var Ɣ=Ɔ.GetInventory(Ɠ);if(Ɣ==null){continue;}for(int Ĭ=0;Ĭ<ŕ.Count;Ĭ++){var ƕ=ŕ[Ĭ];for(int Ɩ=0;Ɩ<ƕ
.InventoryCount;Ɩ++){var ų=ƕ.GetInventory(Ɩ);if(ų==null||ų==Ɣ){continue;}if(!ų.IsConnectedTo(Ɣ)){continue;}double Ɨ=ƒ;if(
!Ƙ(ų,Ɣ,ref Ɨ)){continue;}return true;}}}return false;}bool Ƙ(IMyInventory ų,IMyInventory Ų,ref double ƒ){if(ƒ<=0.0001||ų
==null||Ų==null){return false;}if(!ų.IsConnectedTo(Ų)){return false;}var ƙ=new List<MyInventoryItem>();ų.GetItems(ƙ);for(
int ó=ƙ.Count-1;ó>=0;ó--){var ƚ=ƙ[ó];if(!ƛ(ƚ.Type)){continue;}double Ɯ=(double)ƚ.Amount;double Ɲ=Ɯ;if(Ɲ>ƒ){Ɲ=ƒ;}if(Ɲ<=
0.0001){continue;}MyFixedPoint ƞ=(MyFixedPoint)Ɲ;if(ƞ<=(MyFixedPoint)0){continue;}if(!ų.CanTransferItemTo(Ų,Œ)){continue;}if(ų
.TransferItemTo(Ų,ó,null,true,ƞ)){ƒ-=Ɲ;return true;}if(Ɯ<=ƒ+0.0001&&ų.TransferItemTo(Ų,ó,null,true,null)){ƒ-=Ɯ;return
true;}}return false;}R ť(){Ž();double ƈ=Ŗ;double Ɵ=ŗ;double ƕ=Ř;double Ơ=ƈ+Ɵ+ƕ;var ž=E.q;bool ơ=ž!=null&&ƕ<ž.Ć;int Ű=œ.Count
;int ű=Ŕ.Count;double ƣ=Ƣ(ž);double ƥ=Ƥ(ž);double Ƨ=Ʀ();float ƨ=(float)Ơ;float Ʃ=(float)ƕ;float ƪ=(float)(ƣ+ƥ+Ƨ);float Ŭ;
if(ƪ>0f){Ŭ=ƫ.Ƭ(ƨ/ƪ,0f,1f);}else{Ŭ=0f;}float ŭ=ƣ>0?Ň(ƈ/ƣ):0f;float Ů=ƥ>0?Ň(Ɵ/ƥ):0f;float ů=Ƨ>0?Ň(ƕ/Ƨ):0f;return new R{Ũ=ƨ,ũ
=(float)ƈ,Ū=(float)Ɵ,ū=Ʃ,Ŭ=Ŭ,ŭ=ŭ,Ů=Ů,ů=ů,Ű=Ű,ű=ű,è=ơ};}private static float Ň(double Á){if(Á<=0){return 0f;}if(Á>=1){
return 1f;}return(float)Á;}double Ƣ(A ž){if(ž==null){return 0;}double ƭ=0;for(int ó=0;ó<œ.Count;ó++){bool Ɛ=œ[ó].CubeGrid.
GridSizeEnum==MyCubeSize.Large;ƭ+=Ɛ?ž.Ă:ž.ă;}return ƭ;}double Ƥ(A ž){if(ž==null){return 0;}double ƭ=0;for(int ó=0;ó<Ŕ.Count;ó++){
bool Ɛ=Ŕ[ó].CubeGrid.GridSizeEnum==MyCubeSize.Large;ƭ+=Ɛ?ž.Ą:ž.ą;}return ƭ;}double Ʀ(){double ƭ=0;for(int Ĭ=0;Ĭ<ŕ.Count;Ĭ++)
{var Ƒ=ŕ[Ĭ]as IMyInventoryOwner;if(Ƒ==null){continue;}for(int ó=0;ó<Ƒ.InventoryCount;ó++){var Ʈ=Ƒ.GetInventory(ó);if(Ʈ==
null){continue;}ƭ+=(double)Ʈ.GetItemAmount(Œ);ƭ+=Ư(Ʈ);}}return ƭ;}private static double Ư(IMyInventory Ʈ){if(Ʈ==null||Ʈ.
MaxVolume.RawValue<=0){return 0;}const double ư=2e9;double Ʊ=0;double Ʋ=ư;for(int Ƴ=0;Ƴ<40;Ƴ++){double ƴ=(Ʊ+Ʋ)*0.5;if(ƴ<=0){Ʋ=ƴ;
continue;}MyFixedPoint ƞ=(MyFixedPoint)ƴ;if(Ʈ.CanItemsBeAdded(ƞ,Œ)){Ʊ=ƴ;}else{Ʋ=ƴ;}}return Ʊ;}private static bool š(){return E.p
!=null&&E.p.Me!=null&&E.p.GridTerminalSystem!=null;}private static string Ƶ(){return E.q!=null&&!string.IsNullOrEmpty(E.q.
õ)?E.q.õ:"[Manual]";}private static string ƶ(){return E.q!=null&&!string.IsNullOrEmpty(E.q.ĝ)?E.q.ĝ:"[Irrigator]";}
private static bool Ź(IMyTerminalBlock Ə){if(Ə==null){return false;}string Ʒ=Ƶ();if(string.IsNullOrEmpty(Ʒ)){return false;}
string ú=Ə.CustomName;return û.ü(ú,Ʒ);}private static bool Ÿ(IMyTerminalBlock ø){var Ƒ=ø as IMyInventoryOwner;if(Ƒ==null||Ƒ.
InventoryCount<=0){return false;}string Ʒ=ƶ();if(string.IsNullOrEmpty(Ʒ)){return false;}string ú=ø.CustomName;return û.ü(ú,Ʒ);}private
static bool ƛ(MyItemType Ƹ){if(Ƹ.SubtypeId!="Ice"){return false;}string ƹ=Ƹ.TypeId.ToString();return ƹ.IndexOf("Ore",
StringComparison.Ordinal)>=0;}double ź(){double ƭ=0;for(int ó=0;ó<œ.Count;ó++){var Ƒ=œ[ó]as IMyInventoryOwner;if(Ƒ!=null){ƭ+=ƌ(Ƒ);}}
return ƭ;}double Ż(){double ƭ=0;for(int ó=0;ó<Ŕ.Count;ó++){var Ƒ=Ŕ[ó]as IMyInventoryOwner;if(Ƒ!=null){ƭ+=ƌ(Ƒ);}}return ƭ;}
double ż(){double ƭ=0;for(int Ĭ=0;Ĭ<ŕ.Count;Ĭ++){var ƕ=ŕ[Ĭ];var Ƒ=ƕ as IMyInventoryOwner;if(Ƒ!=null){ƭ+=ƌ(Ƒ);}}return ƭ;}
private static double ƌ(IMyInventoryOwner Ƒ){if(Ƒ==null){return 0;}double ƭ=0;for(int ó=0;ó<Ƒ.InventoryCount;ó++){var Ʈ=Ƒ.
GetInventory(ó);if(Ʈ!=null){ƭ+=(double)Ʈ.GetItemAmount(Œ);}}return ƭ;}}public class E{public static MyGridProgram p;public static A
q;private const double ƺ=0.9;private const string ƻ="[Ore]",Ƽ="[Ingot]",ƽ="[Component]",ƾ="[Ammo]",ƿ="[Tool]",ǀ=
"[Bottle]",ǁ="[Cargo]";private static readonly HashSet<string>ǂ=new HashSet<string>(StringComparer.OrdinalIgnoreCase){"Iron",
"Nickel","Cobalt","Silicon","Magnesium","Silver","Gold","Platinum","Uranium","Stone","Ice"};public static bool ĵ(string ǃ){
return!string.IsNullOrEmpty(ǃ)&&ǂ.Contains(ǃ);}private static readonly HashSet<string>Ǆ=new HashSet<string>(StringComparer.
OrdinalIgnoreCase){"Iron","Nickel","Cobalt","Silicon","Magnesium","Silver","Gold","Platinum","Uranium"},ǅ=new HashSet<string>(
StringComparer.OrdinalIgnoreCase){"SteelPlate","InteriorPlate","ConstructionComponent","SmallTube","LargeTube","Motor","Computer",
"MetalGrid","Display","BulletproofGlass","MedicalComponents","PowerCell","RadioCommunicationComponent","ReactorComponents",
"ThrustModule","GravityGeneratorComponents","Superconductor","Girder","DetectorComponents","Explosives","SolarCell",
"TargetingComputerComponent","PistonMechanism","RotorPart","ArmorPanel","WelderComponent","HandDrillComponent","HydrogenEngineComponent"},ǆ=new
HashSet<string>(StringComparer.OrdinalIgnoreCase){"NATO_25x184mm","NATO_5p56x45mm","Missile200mm","LargeCaliberAmmo",
"MediumCaliberAmmo","AutocannonAmmo","RapidFireAutomaticGunAmmo","Rocket200mm","FlareGunMagazine","AutomaticRifleGun_Mag_20rd",
"ElitePistolMagazine","FullAutoPistolMagazine","PistolMagazine","SemiAutoPistolMagazine","AdvancedPistolMagazine","MilestonePistolMagazine",
"AutomaticRifleGun_Mag_40rd","RapidFireAutomaticGun_Mag_150rd","RapidFireAutomaticGun_Mag_560rd","ArtilleryShell200mm","ArtilleryShell100mm",
"AssaultCraftAmmoMassDriver","Cannon750mmAmmo","AssaultCannonAmmo120mm"},Ǉ=new HashSet<string>(StringComparer.OrdinalIgnoreCase){"WelderItem",
"Welder2Item","Welder3Item","Welder4Item","AngleGrinderItem","AngleGrinder2Item","AngleGrinder3Item","AngleGrinder4Item",
"HandDrillItem","HandDrill2Item","HandDrill3Item","HandDrill4Item"},ǈ=new HashSet<string>(StringComparer.OrdinalIgnoreCase){
"OxygenBottle","HydrogenBottle"};private readonly List<IMyAssembler>ǉ=new List<IMyAssembler>();private readonly List<IMyRefinery>Ǌ=new
List<IMyRefinery>();private readonly List<IMyInventory>ǋ=new List<IMyInventory>(),ǌ=new List<IMyInventory>(),Ǎ=new List<
IMyInventory>(),ǎ=new List<IMyInventory>(),Ǐ=new List<IMyInventory>(),ǐ=new List<IMyInventory>(),Ǒ=new List<IMyInventory>();private
readonly List<IMyTerminalBlock>ǒ=new List<IMyTerminalBlock>();private readonly List<IMyCargoContainer>Ǔ=new List<
IMyCargoContainer>();private readonly HashSet<string>ǔ=new HashSet<string>(StringComparer.OrdinalIgnoreCase);private readonly List<string
>Ǖ=new List<string>();private const int ř=38000;int Ś;byte ś=4,ǖ;N Ǘ=new N();private readonly List<MyInventoryItem>ǘ=new
List<MyInventoryItem>();private readonly StringBuilder Ǚ=new StringBuilder(96);public List<string>É{get{return Ǖ;}}public
bool Ã(){return ś==4;}public static void Ä(List<IMyTerminalBlock>ŝ,List<IMyCargoContainer>ǚ,List<IMyRefinery>Ǜ,List<
IMyGasTank>ǜ,List<IMyGasGenerator>ǝ,List<IMyBatteryBlock>Ǟ){ǚ.Clear();Ǜ.Clear();ǜ.Clear();ǝ.Clear();Ǟ.Clear();if(ŝ==null||p==null
||p.Me==null){return;}for(int ó=0;ó<ŝ.Count;ó++){var Ə=ŝ[ó];if(Ə==null||!Ə.IsSameConstructAs(p.Me)){continue;}if(ǟ(Ə)){
continue;}var Ǡ=Ə as IMyBatteryBlock;if(Ǡ!=null){Ǟ.Add(Ǡ);continue;}var ǡ=Ə as IMyGasTank;if(ǡ!=null){ǜ.Add(ǡ);continue;}var Ǣ=Ə
as IMyGasGenerator;if(Ǣ!=null){ǝ.Add(Ǣ);continue;}var Ƒ=Ə as IMyInventoryOwner;if(Ƒ==null||Ƒ.InventoryCount<=0){continue;}
var ƕ=Ə as IMyCargoContainer;if(ƕ!=null){ǚ.Add(ƕ);continue;}var ǣ=Ə as IMyRefinery;if(ǣ!=null){Ǜ.Add(ǣ);}}}private readonly
Dictionary<string,Ǥ>ǥ=new Dictionary<string,Ǥ>(StringComparer.Ordinal);struct Ǥ{public MyFixedPoint Ǧ;public string ǧ,Ǩ;}public
bool Ç(List<IMyTerminalBlock>ŝ,List<IMyCargoContainer>Ş,List<IMyRefinery>ǩ,ref N Ǫ,ref X ǫ){if(!Ǭ()){ś=4;ǭ(ref Ǫ);Ǯ(ref ǫ);
return true;}if(ś==4){Ǖ.Clear();ǔ.Clear();ǯ(Ş,ǩ);ś=0;Ś=0;}if(ś==0){if(!ǰ(ŝ)){return false;}ś=1;ǖ=0;Ś=0;return false;}if(ś==1){
if(!Ǳ()){return false;}ǲ();ś=2;Ś=0;return false;}if(ś==2){if(!ǳ()){return false;}ś=3;Ś=0;ǥ.Clear();return false;}if(ś==3){
if(!Ǵ()){return false;}ǵ(Ǫ);Ƕ(ǫ);ś=4;return true;}ś=4;return true;}private static void ǭ(ref N ŧ){ŧ=new N();}private
static void Ǯ(ref X ŧ){ŧ.Ƿ=new string[0];ŧ.Ǹ=new float[0];ŧ.ǹ=new string[0];}void ǯ(List<IMyCargoContainer>Ş,List<IMyRefinery>
ǩ){ǒ.Clear();Ǔ.Clear();ǉ.Clear();Ǌ.Clear();ǋ.Clear();ǌ.Clear();Ǎ.Clear();ǎ.Clear();Ǐ.Clear();ǐ.Clear();Ǒ.Clear();if(Ş!=
null){for(int ó=0;ó<Ş.Count;ó++){var ƕ=Ş[ó];if(ƕ==null){continue;}ǒ.Add(ƕ);Ǔ.Add(ƕ);Ǻ(ƕ);}}if(ǩ!=null){for(int ó=0;ó<ǩ.Count
;ó++){var ǣ=ǩ[ó];if(ǣ==null){continue;}ǒ.Add(ǣ);Ǌ.Add(ǣ);}}}bool ǰ(List<IMyTerminalBlock>ŝ){if(ŝ==null){return true;}for(
int ó=Ś;ó<ŝ.Count;ó++){if(p.Runtime.CurrentInstructionCount>ř){Ś=ó;return false;}var Ə=ŝ[ó];if(!ǻ(Ə)||Ź(Ə)){continue;}if(Ə
as IMyCargoContainer!=null||Ə as IMyRefinery!=null){continue;}ǒ.Add(Ə);var ù=Ə as IMyAssembler;if(ù!=null){ǉ.Add(ù);}}
return true;}private static IMyInventory ǽ(IMyRefinery ǣ){if(ǣ==null){return null;}if(ǣ.InventoryCount>=2){var Ǽ=ǣ.
GetInventory(1);if(Ǽ!=null){return Ǽ;}}return ǣ.OutputInventory;}bool Ǳ(){const int Ǿ=5;int ǿ=0;if(ǖ==0){for(int Ȁ=Ś;Ȁ<Ǔ.Count;Ȁ++){
if(p.Runtime.CurrentInstructionCount>ř){Ś=Ȁ;return false;}var ƕ=Ǔ[Ȁ];if(ƕ==null||Ź(ƕ)){continue;}string ȁ=ƕ.CustomName??
string.Empty;if(!û.ü(ȁ,ƽ)){continue;}for(int Ɩ=0;Ɩ<ƕ.InventoryCount;Ɩ++){var Ʈ=ƕ.GetInventory(Ɩ);if(Ʈ==null){continue;}ǘ.Clear
();Ʈ.GetItems(ǘ);for(int Ȃ=ǘ.Count-1;Ȃ>=0&&ǿ<Ǿ;Ȃ--){if(p.Runtime.CurrentInstructionCount>ř){Ś=Ȁ;return false;}if(!ȃ(ǘ[Ȃ].
Type)){continue;}if(Ȅ(Ʈ,Ȃ)){ǿ++;}}}}ȅ(0,ǿ);ǖ=1;Ś=0;return false;}if(ǖ==1){for(int Ȁ=Ś;Ȁ<Ǔ.Count;Ȁ++){if(p.Runtime.
CurrentInstructionCount>ř){Ś=Ȁ;return false;}var ƕ=Ǔ[Ȁ];if(ƕ==null||Ź(ƕ)){continue;}string ȁ=ƕ.CustomName??string.Empty;if(!û.ü(ȁ,Ƽ)){continue;
}for(int Ɩ=0;Ɩ<ƕ.InventoryCount;Ɩ++){var Ʈ=ƕ.GetInventory(Ɩ);if(Ʈ==null){continue;}ǘ.Clear();Ʈ.GetItems(ǘ);for(int Ȃ=ǘ.
Count-1;Ȃ>=0&&ǿ<Ǿ;Ȃ--){if(p.Runtime.CurrentInstructionCount>ř){Ś=Ȁ;return false;}if(!Ȇ(ǘ[Ȃ].Type)){continue;}if(Ȅ(Ʈ,Ȃ)){ǿ++;}
}}}ȅ(1,ǿ);ǖ=2;Ś=0;return false;}if(ǖ==2){for(int ȇ=Ś;ȇ<ǉ.Count;ȇ++){if(p.Runtime.CurrentInstructionCount>ř){Ś=ȇ;return
false;}var ù=ǉ[ȇ];var Ȉ=ù.OutputInventory;if(Ȉ==null){continue;}ǘ.Clear();Ȉ.GetItems(ǘ);for(int Ȃ=ǘ.Count-1;Ȃ>=0&&ǿ<Ǿ;Ȃ--){if
(p.Runtime.CurrentInstructionCount>ř){Ś=ȇ;return false;}if(!ȃ(ǘ[Ȃ].Type)){continue;}if(ȉ(Ȉ,Ȃ)){ǿ++;}}}ȅ(2,ǿ);ǖ=3;Ś=0;
return false;}if(ǖ==3){const int Ȋ=20;for(int ł=Ś;ł<Ǌ.Count;ł++){if(p.Runtime.CurrentInstructionCount>ř){Ś=ł;return false;}var
ǣ=Ǌ[ł];var Ȉ=ǽ(ǣ);if(Ȉ==null){continue;}IMyInventory ȋ=ǣ.InventoryCount>0?ǣ.GetInventory(0):null;ǘ.Clear();Ȉ.GetItems(ǘ);
for(int Ȃ=ǘ.Count-1;Ȃ>=0&&ǿ<Ȋ;Ȃ--){if(p.Runtime.CurrentInstructionCount>ř){Ś=ł;return false;}if(ȉ(Ȉ,Ȃ,ȋ)){ǿ++;}}}ȅ(3,ǿ);ǖ=4
;Ś=0;return false;}for(int ȇ=Ś;ȇ<ǉ.Count;ȇ++){if(p.Runtime.CurrentInstructionCount>ř){Ś=ȇ;return false;}var ù=ǉ[ȇ];var Ȍ=
ù.InputInventory;if(Ȍ==null||Ȍ.MaxVolume.RawValue<=0){continue;}while(ǿ<Ǿ){double ȍ=(double)Ȍ.CurrentVolume.RawValue/(
double)Ȍ.MaxVolume.RawValue;if(ȍ<ƺ){break;}if(p.Runtime.CurrentInstructionCount>ř){Ś=ȇ;return false;}ǘ.Clear();Ȍ.GetItems(ǘ);
bool Ȏ=false;for(int Ȃ=ǘ.Count-1;Ȃ>=0;Ȃ--){if(!Ȇ(ǘ[Ȃ].Type)){continue;}if(Ȅ(Ȍ,Ȃ)){ǿ++;Ȏ=true;break;}}if(!Ȏ){break;}ȍ=(double
)Ȍ.CurrentVolume.RawValue/(double)Ȍ.MaxVolume.RawValue;if(ȍ<ƺ){break;}}double ȏ=Ȍ.MaxVolume.RawValue>0?(double)Ȍ.
CurrentVolume.RawValue/(double)Ȍ.MaxVolume.RawValue:0;if(ȏ>=ƺ&&ǿ>=Ǿ){Ś=ȇ;return false;}}ȅ(4,ǿ);return true;}void ǲ(){Ǘ=new N();float
Ȑ=0f;float ê=0f;for(int Ĭ=0;Ĭ<Ǔ.Count;Ĭ++){var ƕ=Ǔ[Ĭ];if(Ź(ƕ)){continue;}for(int Ɩ=0;Ɩ<ƕ.InventoryCount;Ɩ++){var Ʈ=ƕ.
GetInventory(Ɩ);Ȑ+=(float)Ʈ.CurrentVolume;ê+=(float)Ʈ.MaxVolume;}}Ǘ.Ȑ=Ȑ;Ǘ.ê=ê;Ǘ.Ú=ê>0.0001f?(Ȑ/ê)*100f:0f;}bool ǳ(){for(int ø=Ś;ø<ǒ.
Count;ø++){if(p.Runtime.CurrentInstructionCount>ř){Ś=ø;return false;}var Ə=ǒ[ø];if(Ź(Ə)){continue;}var Ƒ=Ə as
IMyInventoryOwner;if(Ƒ==null){continue;}for(int Ɩ=0;Ɩ<Ƒ.InventoryCount;Ɩ++){var Ʈ=Ƒ.GetInventory(Ɩ);ȑ(Ʈ,ref Ǘ);}}return true;}bool Ǵ(){
for(int ø=Ś;ø<ǒ.Count;ø++){if(p.Runtime.CurrentInstructionCount>ř){Ś=ø;return false;}var Ə=ǒ[ø];if(Ź(Ə)){continue;}var Ƒ=Ə
as IMyInventoryOwner;if(Ƒ==null){continue;}for(int Ɩ=0;Ɩ<Ƒ.InventoryCount;Ɩ++){var Ʈ=Ƒ.GetInventory(Ɩ);var ƙ=new List<
MyInventoryItem>();Ʈ.GetItems(ƙ);for(int ó=0;ó<ƙ.Count;ó++){var ƚ=ƙ[ó];if(!Ȓ(ƚ.Type)){continue;}string ŏ=ƚ.Type.ToString();string Ȕ=ȓ(ƚ
.Type);Ǥ ȕ;if(ǥ.TryGetValue(ŏ,out ȕ)){ȕ.Ǧ+=ƚ.Amount;ǥ[ŏ]=ȕ;}else{ǥ[ŏ]=new Ǥ{Ǧ=ƚ.Amount,ǧ=Ȕ,Ǩ=ƚ.Type.SubtypeId};}}}}return
true;}void ǵ(N ŧ){ŧ.Ȗ=Ǘ.Ȗ;ŧ.ȗ=Ǘ.ȗ;ŧ.Ș=Ǘ.Ș;ŧ.ș=Ǘ.ș;ŧ.Ț=Ǘ.Ț;ŧ.ț=Ǘ.ț;ŧ.Ȝ=Ǘ.Ȝ;ŧ.ȝ=Ǘ.ȝ;ŧ.Ȟ=Ǘ.Ȟ;ŧ.ȟ=Ǘ.ȟ;ŧ.Ƞ=Ǘ.Ƞ;ŧ.ȡ=Ǘ.ȡ;ŧ.Ȣ=Ǘ.Ȣ;ŧ.
ȣ=Ǘ.ȣ;ŧ.Ȥ=Ǘ.Ȥ;ŧ.ȥ=Ǘ.ȥ;ŧ.Ȧ=Ǘ.Ȧ;ŧ.ȧ=Ǘ.ȧ;ŧ.Ȩ=Ǘ.Ȩ;ŧ.ȩ=Ǘ.ȩ;ŧ.Ȫ=Ǘ.Ȫ;ŧ.ȫ=Ǘ.ȫ;ŧ.Ȭ=Ǘ.Ȭ;ŧ.ȭ=Ǘ.ȭ;ŧ.Ȑ=Ǘ.Ȑ;ŧ.ê=Ǘ.ê;ŧ.Ú=Ǘ.Ú;}void Ƕ(X á
){int Â=ǥ.Count;if(Â==0){á.Ƿ=new string[0];á.Ǹ=new float[0];á.ǹ=new string[0];return;}var Ȯ=new string[Â];var ȯ=new float
[Â];var Ȱ=new string[Â];int ȱ=0;foreach(var Ō in ǥ){var ȕ=Ō.Value;Ȯ[ȱ]=Ȳ.ȳ(ȕ.Ǩ);ȯ[ȱ]=(float)ȕ.Ǧ;Ȱ[ȱ]=Ȳ.ȳ(ȕ.ǧ);ȱ++;}á.Ƿ=Ȯ;
á.Ǹ=ȯ;á.ǹ=Ȱ;}void ȅ(byte ȴ,int ǿ){if(ǿ<=0||q==null||!q.Ĝ||p==null){return;}Ǚ.Clear();Ǚ.Append("INV ex=");Ǚ.Append(ȴ);Ǚ.
Append(" n=");Ǚ.Append(ǿ);p.Echo(Ǚ.ToString());}bool Ȅ(IMyInventory ȵ,int ȶ){ǘ.Clear();ȵ.GetItems(ǘ);if(ȶ<0||ȶ>=ǘ.Count){
return false;}for(int ŧ=0;ŧ<Ǒ.Count;ŧ++){var Ų=Ǒ[ŧ];if(Ų==null||Ų==ȵ){continue;}if(ȷ(ȵ,ȶ,Ų)){return true;}}return false;}bool
ȉ(IMyInventory ȵ,int ȶ,IMyInventory ȸ=null){ǘ.Clear();ȵ.GetItems(ǘ);if(ȶ<0||ȶ>=ǘ.Count){return false;}MyItemType ȹ=ǘ[ȶ].
Type;Ⱥ ȼ=Ȼ(ȹ);List<IMyInventory>Ƚ;switch(ȼ){case Ⱥ.Ⱦ:Ƚ=ǋ;break;case Ⱥ.ȿ:Ƚ=ǌ;break;case Ⱥ.ɀ:Ƚ=Ǎ;break;case Ⱥ.Ɂ:Ƚ=ǎ;break;case
Ⱥ.ɂ:Ƚ=Ǐ;break;case Ⱥ.Ƀ:Ƚ=ǐ;break;default:Ƚ=Ǒ;break;}for(int ŧ=0;ŧ<Ƚ.Count;ŧ++){var Ų=Ƚ[ŧ];if(Ų==null||Ų==ȵ||Ų==ȸ){
continue;}if(ȷ(ȵ,ȶ,Ų)){return true;}}if(ȼ==Ⱥ.Ʉ){return false;}for(int ŧ=0;ŧ<Ǒ.Count;ŧ++){var Ų=Ǒ[ŧ];if(Ų==null||Ų==ȵ||Ų==ȸ){
continue;}if(ȷ(ȵ,ȶ,Ų)){return true;}}return false;}private static bool ȷ(IMyInventory ȵ,int ȶ,IMyInventory Ų){if(!ȵ.
IsConnectedTo(Ų)){return false;}return ȵ.TransferItemTo(Ų,ȶ,null,true,null);}void ȑ(IMyInventory Ʈ,ref N á){var ƙ=new List<
MyInventoryItem>();Ʈ.GetItems(ƙ);for(int ó=0;ó<ƙ.Count;ó++){var ƚ=ƙ[ó];var Ƹ=ƚ.Type;string Ʌ=Ƹ.SubtypeId;float ƞ=(float)ƚ.Amount;if(Ɇ(Ƹ
)){if(ǂ.Contains(Ʌ)){ɇ(ref á,Ʌ,ƞ);}else if(ǔ.Add(Ʌ)){Ǖ.Add(Ʌ);}continue;}if(Ȇ(Ƹ)){if(Ǆ.Contains(Ʌ)){Ɉ(ref á,Ʌ,ƞ);}
continue;}if(ȃ(Ƹ)){á.Ȫ+=ƞ;continue;}if(ɉ(Ƹ)){á.ȫ+=ƞ;continue;}if(Ɋ(Ƹ)){á.Ȭ+=ƞ;continue;}if(ɋ(Ƹ)){á.ȭ+=ƞ;}}}private static void ɇ
(ref N á,string Ʌ,float ƞ){switch(Ʌ.ToUpperInvariant()){case"IRON":á.Ȗ+=ƞ;break;case"NICKEL":á.ȗ+=ƞ;break;case"COBALT":á.
Ș+=ƞ;break;case"SILICON":á.ș+=ƞ;break;case"MAGNESIUM":á.Ț+=ƞ;break;case"SILVER":á.ț+=ƞ;break;case"GOLD":á.Ȝ+=ƞ;break;case
"PLATINUM":á.ȝ+=ƞ;break;case"URANIUM":á.Ȟ+=ƞ;break;case"STONE":á.ȟ+=ƞ;break;case"ICE":á.Ƞ+=ƞ;break;}}private static void Ɉ(ref N á
,string Ʌ,float ƞ){switch(Ʌ.ToUpperInvariant()){case"IRON":á.ȡ+=ƞ;break;case"NICKEL":á.Ȣ+=ƞ;break;case"COBALT":á.ȣ+=ƞ;
break;case"SILICON":á.Ȥ+=ƞ;break;case"MAGNESIUM":á.ȥ+=ƞ;break;case"SILVER":á.Ȧ+=ƞ;break;case"GOLD":á.ȧ+=ƞ;break;case
"PLATINUM":á.Ȩ+=ƞ;break;case"URANIUM":á.ȩ+=ƞ;break;}}bool Ȓ(MyItemType Ƹ){return true;}private static string ȓ(MyItemType Ƹ){if(Ɇ(
Ƹ)){return"Ore";}if(Ȇ(Ƹ)){return"Ingot";}if(ȃ(Ƹ)){return"Component";}if(ɉ(Ƹ)){return"Ammo";}if(Ɋ(Ƹ)){return"Tool";}if(ɋ(Ƹ
)){return"Bottle";}return"Other";}enum Ⱥ{Ⱦ,ȿ,ɀ,Ɂ,ɂ,Ƀ,Ʉ}private static Ⱥ Ȼ(MyItemType Ƹ){if(Ɇ(Ƹ)){return Ⱥ.Ⱦ;}if(Ȇ(Ƹ)){
return Ⱥ.ȿ;}if(ȃ(Ƹ)){return Ⱥ.ɀ;}if(ɉ(Ƹ)){return Ⱥ.Ɂ;}if(Ɋ(Ƹ)){return Ⱥ.ɂ;}if(ɋ(Ƹ)){return Ⱥ.Ƀ;}return Ⱥ.Ʉ;}private static
bool Ɇ(MyItemType Ƹ){string ƹ=Ƹ.TypeId.ToString();return ƹ.IndexOf("Ore",StringComparison.Ordinal)>=0;}private static bool Ȇ
(MyItemType Ƹ){return Ƹ.TypeId.ToString().IndexOf("Ingot",StringComparison.Ordinal)>=0;}private static bool ȃ(MyItemType
Ƹ){return Ƹ.TypeId.ToString().IndexOf("Component",StringComparison.Ordinal)>=0;}private static bool ɉ(MyItemType Ƹ){
string ƹ=Ƹ.TypeId.ToString();return ƹ.IndexOf("Ammo",StringComparison.Ordinal)>=0||ƹ.IndexOf("Magazine",StringComparison.
Ordinal)>=0;}private static bool Ɋ(MyItemType Ƹ){string ƹ=Ƹ.TypeId.ToString();if(ƹ.IndexOf("PhysicalGun",StringComparison.
Ordinal)>=0){return true;}if(ƹ.IndexOf("Welder",StringComparison.Ordinal)>=0){return true;}if(ƹ.IndexOf("Drill",
StringComparison.Ordinal)>=0&&ƹ.IndexOf("Component",StringComparison.Ordinal)<0){return true;}if(ƹ.IndexOf("Grinder",StringComparison.
Ordinal)>=0){return true;}return false;}private static bool ɋ(MyItemType Ƹ){string ƹ=Ƹ.TypeId.ToString();return ƹ.IndexOf(
"OxygenContainer",StringComparison.Ordinal)>=0||ƹ.IndexOf("GasContainer",StringComparison.Ordinal)>=0;}bool Ǭ(){return p!=null&&p.Me!=
null&&p.GridTerminalSystem!=null;}private static bool ǟ(IMyTerminalBlock Ə){if(Ə==null){return false;}string Ʒ=q!=null&&!
string.IsNullOrEmpty(q.õ)?q.õ:"[Manual]";if(string.IsNullOrEmpty(Ʒ)){return false;}string ú=Ə.CustomName;return û.ü(ú,Ʒ);}bool
Ź(IMyTerminalBlock Ə){return ǟ(Ə);}bool ǻ(IMyTerminalBlock Ə){if(Ə==null||!Ə.IsSameConstructAs(p.Me)){return false;}var Ƒ
=Ə as IMyInventoryOwner;return Ƒ!=null&&Ƒ.InventoryCount>0;}void Ǻ(IMyCargoContainer ƕ){string ú=ƕ.CustomName??string.
Empty;bool Ɍ=û.ü(ú,ƻ);bool ɍ=û.ü(ú,Ƽ);bool Ɏ=û.ü(ú,ƽ);bool ɏ=û.ü(ú,ƾ);bool ɐ=û.ü(ú,ƿ);bool ɑ=û.ü(ú,ǀ);bool ɒ=û.ü(ú,ǁ);bool ɓ=
Ɍ||ɍ||Ɏ||ɏ||ɐ||ɑ;for(int Ɩ=0;Ɩ<ƕ.InventoryCount;Ɩ++){var Ʈ=ƕ.GetInventory(Ɩ);if(Ɍ){ǋ.Add(Ʈ);}if(ɍ){ǌ.Add(Ʈ);}if(Ɏ){Ǎ.Add(
Ʈ);}if(ɏ){ǎ.Add(Ʈ);}if(ɐ){Ǐ.Add(Ʈ);}if(ɑ){ǐ.Add(Ʈ);}if(ɒ||!ɓ){Ǒ.Add(Ʈ);}}}}public class K{private const double ɔ=1e-9;
private readonly List<IMyBatteryBlock>ɕ=new List<IMyBatteryBlock>();private readonly List<IMyReactor>ɖ=new List<IMyReactor>();
private readonly List<IMyPowerProducer>ɗ=new List<IMyPowerProducer>(),ɘ=new List<IMyPowerProducer>();double ə;double ɚ,ɛ,ɜ,ɝ,ɞ,
ɟ,ɠ,ɡ,ɢ,ɣ;private const int ř=38000;int Ś;byte ś=4;bool ɤ;public bool Í(List<IMyTerminalBlock>ŝ,List<IMyBatteryBlock>ɥ,
ref T á){if(!Ǭ()){ś=4;ɦ(ref á);return true;}if(ś==4){ɕ.Clear();ɖ.Clear();ɗ.Clear();ɘ.Clear();ɤ=false;ś=0;Ś=0;}if(ś==0){if(!
ɧ(ŝ,ɥ)){return false;}ɨ();ɩ();ɪ();var ɬ=ɫ();ɭ(á,ɬ);ś=4;return true;}return true;}private static void ɦ(ref T ŧ){ŧ.ɮ=0f;ŧ.
ɯ=0f;ŧ.ɰ=0f;ŧ.ɱ=0f;ŧ.ɲ=0f;ŧ.ɳ=0f;ŧ.ɴ=0f;ŧ.ɵ=0f;ŧ.ɶ=0f;ŧ.ɷ=0f;ŧ.ɸ=0;ŧ.ɹ=0;ŧ.ɺ=0;ŧ.Û=false;}private static void ɭ(T Ų,T ų){
Ų.ɮ=ų.ɮ;Ų.ɯ=ų.ɯ;Ų.ɰ=ų.ɰ;Ų.ɱ=ų.ɱ;Ų.ɲ=ų.ɲ;Ų.ɳ=ų.ɳ;Ų.ɴ=ų.ɴ;Ų.ɵ=ų.ɵ;Ų.ɶ=ų.ɶ;Ų.ɷ=ų.ɷ;Ų.ɸ=ų.ɸ;Ų.ɹ=ų.ɹ;Ų.ɺ=ų.ɺ;Ų.Û=ų.Û;}bool ɧ(
List<IMyTerminalBlock>ŝ,List<IMyBatteryBlock>ɥ){if(ŝ==null){return true;}var ɻ=E.p.Me;if(!ɤ&&ɥ!=null){for(int ɼ=0;ɼ<ɥ.Count;
ɼ++){var ɽ=ɥ[ɼ];if(ɽ!=null&&ɽ.IsSameConstructAs(ɻ)&&!Ź(ɽ)){ɕ.Add(ɽ);}}ɤ=true;}for(int ó=Ś;ó<ŝ.Count;ó++){if(E.p.Runtime.
CurrentInstructionCount>ř){Ś=ó;return false;}var Ə=ŝ[ó];if(!Ə.IsSameConstructAs(ɻ)||Ź(Ə)){continue;}if(Ə is IMyBatteryBlock){continue;}var ɾ=Ə
as IMyReactor;if(ɾ!=null){ɖ.Add(ɾ);continue;}var ɿ=Ə as IMyPowerProducer;if(ɿ!=null){if(ʀ(ɿ)){ɗ.Add(ɿ);}else if(ʁ(ɿ)){ɘ.
Add(ɿ);}}}return true;}private static bool ʁ(IMyPowerProducer ø){var ʂ=ø as IMyTerminalBlock;if(ʂ==null){return false;}
string ʃ=ʂ.BlockDefinition.ToString();return ʃ.IndexOf("SolarPanel",ʄ.ʅ)>=0;}private static bool ʀ(IMyPowerProducer ø){var ʂ=ø
as IMyTerminalBlock;if(ʂ==null){return false;}string ʃ=ʂ.BlockDefinition.ToString();return ʃ.IndexOf("HydrogenEngine",ʄ.ʅ)
>=0;}void ɨ(){ə=0;ɚ=0;ɛ=0;ɜ=0;ɝ=0;ɞ=0;ɟ=0;ɠ=0;ɡ=0;ɢ=0;ɣ=0;for(int ó=0;ó<ɕ.Count;ó++){var ɽ=ɕ[ó];ə+=ɽ.CurrentStoredPower;ɚ
+=ɽ.MaxStoredPower;ɛ+=ɽ.CurrentInput;ɜ+=ɽ.CurrentOutput;ɠ+=ɽ.MaxInput;ɡ+=ɽ.MaxOutput;}for(int ó=0;ó<ɘ.Count;ó++){ɝ+=ɘ[ó].
CurrentOutput;}for(int ó=0;ó<ɖ.Count;ó++){ɞ+=ɖ[ó].CurrentOutput;ɢ+=ɖ[ó].MaxOutput;}for(int ó=0;ó<ɗ.Count;ó++){ɟ+=ɗ[ó].CurrentOutput;ɣ
+=ɗ[ó].MaxOutput;}}void ɩ(){if(ɕ.Count==0){return;}var ž=E.q;double ʆ=0.25;double ʇ=0.80;if(ž!=null){ʆ=ž.Ē;ʇ=ž.ē;}double ʈ
=0;if(ɚ>ɔ){ʈ=ə/ɚ;}bool ʉ=ʈ<ʆ;bool ʊ=ʈ>ʇ;for(int ó=0;ó<ɕ.Count;ó++){var ɽ=ɕ[ó];if(ʉ){ɽ.ChargeMode=ChargeMode.Recharge;}
else if(ʊ){ɽ.ChargeMode=ChargeMode.Auto;}}}void ɪ(){var ž=E.q;if(ž==null||!ž.Ė){return;}bool ʋ=ɝ<ž.Ĕ;bool ʌ=ɝ>=ž.Ĕ;if(ʋ){for
(int ó=0;ó<ɖ.Count;ó++){ɖ[ó].Enabled=true;}for(int ó=0;ó<ɗ.Count;ó++){ɗ[ó].Enabled=true;}}else if(ʌ){for(int ó=0;ó<ɖ.
Count;ó++){ɖ[ó].Enabled=false;}for(int ó=0;ó<ɗ.Count;ó++){ɗ[ó].Enabled=false;}}}T ɫ(){var á=new T();á.ɮ=(float)ə;á.ɯ=(float)ɚ
;á.ɰ=(float)ɛ;á.ɱ=(float)ɜ;á.ɲ=(float)ɠ;á.ɳ=(float)ɡ;á.ɴ=(float)ɞ;á.ɵ=(float)ɟ;á.ɶ=(float)ɢ;á.ɷ=(float)ɣ;á.ɸ=ɕ.Count;á.ɹ=
ɖ.Count;á.ɺ=ɗ.Count;double ʍ=0;if(ɚ>ɔ){ʍ=ə/ɚ;}double ʎ=0.25;if(E.q!=null){ʎ=E.q.Ē;}á.Û=ɚ>ɔ&&ʍ<ʎ;return á;}bool Ǭ(){return
E.p!=null&&E.p.Me!=null&&E.p.GridTerminalSystem!=null;}bool Ź(IMyTerminalBlock Ə){if(Ə==null){return false;}var ž=E.q;
string Ʒ=ž!=null&&!string.IsNullOrEmpty(ž.õ)?ž.õ:"[Manual]";if(string.IsNullOrEmpty(Ʒ)){return false;}string ú=Ə.CustomName;
return û.ü(ú,Ʒ);}}public sealed class G{private const double ʏ=0.05,ʐ=1000,ʑ=100000,ʒ=50000,ʓ=0.3,ʔ=125000;private readonly
List<IMyRefinery>ʕ=new List<IMyRefinery>();private readonly HashSet<IMyInventory>ʖ=new HashSet<IMyInventory>();List<
IMyTerminalBlock>a;private readonly List<string>ʗ=new List<string>();private readonly Dictionary<string,double>ʘ=new Dictionary<string,
double>(StringComparer.OrdinalIgnoreCase);private readonly List<MyInventoryItem>ʙ=new List<MyInventoryItem>();private readonly
Dictionary<string,int>ʚ=new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase);private readonly List<KeyValuePair<string,int
>>ʛ=new List<KeyValuePair<string,int>>(32);private static readonly int[]ʜ={2000,1000,500,250,100,50};string ʝ=string.
Empty;private const int ř=38000;int Ś,ʞ;byte ʟ=255;Dictionary<string,int>ʠ;private static int ʤ(int ó,int ȇ,int ø){if(ø<=0){
return 0;}if(ȇ<4){return 0;}int Ƹ;if(ȇ<8){int ʡ=(2*ȇ+2)/3;Ƹ=ó<ʡ?0:1;}else{int ʡ=(ȇ+1)/2;int ʢ=ȇ-ʡ;int ʣ=(ʢ+1)/2;if(ó<ʡ){Ƹ=0;}
else if(ó<ʡ+ʣ){Ƹ=1;}else{Ƹ=2;}}if(Ƹ>=ø){Ƹ=ø-1;}return Ƹ;}private static double ʦ(string ʥ){if(ʥ==null){return ʓ;}switch(ʥ.
ToLowerInvariant()){case"iron":case"silicon":return 0.7;case"nickel":return 0.4;case"cobalt":return 0.3;case"magnesium":return 0.007;
case"silver":return 0.1;case"gold":return 0.01;case"platinum":return 0.005;case"uranium":return 0.01;default:return ʓ;}}
private static double ʬ(string ʧ,A ž){if(string.Equals(ʧ,"Stone",ʄ.ʅ)){return ʒ;}double ß;string ý=ʨ(ʧ);if(ý==null||!ʩ(ž,ý,out
ß)||ß<=0){if(!ʩ(ž,"Iron",out ß)||ß<=0){ß=ʔ;}}double ʪ=ʦ(ʧ);if(ʪ<=0){ʪ=ʓ;}double ʫ=ʏ*(ß/ʪ);if(ʫ<ʐ){ʫ=ʐ;}if(ʫ>ʑ){ʫ=ʑ;}
return ʫ;}public bool Ê(List<IMyTerminalBlock>ŝ,List<IMyRefinery>ǩ,ref P á){if(!Ǭ()){ʟ=255;ʭ(á);return true;}if(ʟ==255){a=ŝ;ʮ(
ǩ);A ʯ=E.q;ʰ(ʯ!=null&&ʯ.Ý);ʗ.Clear();Ś=0;ʟ=1;return false;}if(ʟ==1){if(!ʱ()){return false;}ʘ.Clear();Ś=0;ʟ=2;return false
;}if(ʟ==2){if(!ʲ()){return false;}ʳ();ʠ=ʴ();A ž=E.q;bool ʵ=ž!=null&&ž.Ý;if(ʵ){ʞ=0;Ś=0;ʟ=4;}else{ʟ=6;}return false;}if(ʟ==
4){if(!ʶ()){return false;}Ś=0;ʟ=5;return false;}if(ʟ==5){if(!ʷ()){return false;}ʟ=6;return false;}if(ʟ==6){var ɬ=ʸ(ʠ);ʹ(á
,ɬ);ʟ=255;return true;}return true;}private static void ʭ(P ŧ){ŧ.ë=new string[0];ŧ.ʺ=new string[0];ŧ.ʻ=new float[0];ŧ.ʼ=
new string[0];ŧ.ʽ=new float[0];ŧ.ò=new bool[0];ŧ.ñ=new bool[0];ŧ.ʾ=null;ŧ.ʿ=null;}private static void ʹ(P Ų,P ų){Ų.ë=ų.ë;Ų.
ʺ=ų.ʺ;Ų.ʻ=ų.ʻ;Ų.ʼ=ų.ʼ;Ų.ʽ=ų.ʽ;Ų.ò=ų.ò;Ų.ñ=ų.ñ;Ų.ʾ=ų.ʾ;Ų.ʿ=ų.ʿ;}bool ʱ(){if(a==null){return true;}for(int ɼ=Ś;ɼ<a.Count;ɼ
++){if(E.p.Runtime.CurrentInstructionCount>ř){Ś=ɼ;return false;}var Ə=a[ɼ];if(Ź(Ə)){continue;}var Ƒ=Ə as IMyInventoryOwner
;if(Ƒ==null){continue;}for(int ˀ=0;ˀ<Ƒ.InventoryCount;ˀ++){var Ʈ=Ƒ.GetInventory(ˀ);if(Ʈ==null){continue;}var ƙ=new List<
MyInventoryItem>();Ʈ.GetItems(ƙ);for(int ȱ=0;ȱ<ƙ.Count;ȱ++){var Ƹ=ƙ[ȱ].Type;if(!Ɇ(Ƹ)){continue;}string Ʌ=Ƹ.SubtypeId;if(string.Equals(Ʌ
,"Ice",ʄ.ʅ)){continue;}bool ˁ=false;for(int ń=0;ń<ʗ.Count;ń++){if(string.Equals(ʗ[ń],Ʌ,ʄ.ʅ)){ˁ=true;break;}}if(!ˁ){ʗ.Add(
Ʌ);}}}}return true;}bool ʲ(){if(a==null){return true;}for(int ɼ=Ś;ɼ<a.Count;ɼ++){if(E.p.Runtime.CurrentInstructionCount>ř
){Ś=ɼ;return false;}var Ə=a[ɼ];if(Ź(Ə)){continue;}var Ƒ=Ə as IMyInventoryOwner;if(Ƒ==null){continue;}for(int ˀ=0;ˀ<Ƒ.
InventoryCount;ˀ++){var Ʈ=Ƒ.GetInventory(ˀ);if(Ʈ==null){continue;}var ƙ=new List<MyInventoryItem>();Ʈ.GetItems(ƙ);for(int ȱ=0;ȱ<ƙ.
Count;ȱ++){var ƚ=ƙ[ȱ];if(!ˆ(ƚ.Type)){continue;}string Ʌ=ƚ.Type.SubtypeId;double ƞ=(double)ƚ.Amount;double ˇ;if(!ʘ.TryGetValue
(Ʌ,out ˇ)){ˇ=0;}ʘ[Ʌ]=ˇ+ƞ;}}}return true;}bool ʶ(){if(ʗ.Count==0){return true;}int ˈ=0;const int ˉ=20;for(int Ƹ=ʞ;Ƹ<ʗ.
Count&&ˈ<ˉ;Ƹ++){if(E.p.Runtime.CurrentInstructionCount>ř){ʞ=Ƹ;return false;}string ˊ=ʗ[Ƹ];while(ˈ<ˉ&&ˋ(ˊ)){if(E.p.Runtime.
CurrentInstructionCount>ř){ʞ=Ƹ;return false;}ˈ++;}}return true;}bool ʷ(){if(ʗ.Count==0){return true;}int ȇ=ʕ.Count;int ø=ʗ.Count;for(int ó=Ś;ó<
ȇ;ó++){if(E.p.Runtime.CurrentInstructionCount>ř){Ś=ó;return false;}var ł=ʕ[ó];var ˌ=ł.InputInventory;if(ˌ==null){continue
;}string ˍ=ʗ[ʤ(ó,ȇ,ø)];if(ˎ(ł,ˍ)<=50f&&ˏ(ł).RawValue>0){IMyInventory ː;int ˑ;if(ˠ(ˍ,ˌ,out ː,out ˑ)){for(int ń=0;ń<ʜ.
Length;ń++){if(E.p.Runtime.CurrentInstructionCount>ř){Ś=ó;return false;}ː.GetItems(ʙ);if(ˑ>=ʙ.Count){break;}MyItemType ˡ=ʙ[ˑ].
Type;if(!ː.CanTransferItemTo(ˌ,ˡ)){break;}MyFixedPoint ƞ=ʙ[ˑ].Amount;if(ƞ<=(MyFixedPoint)0){break;}MyFixedPoint ˢ=(
MyFixedPoint)ʜ[ń];if(ƞ<ˢ){ˢ=ƞ;}if(ˢ<=(MyFixedPoint)0){continue;}if(ː.TransferItemTo(ˌ,ˑ,null,true,ˢ)){break;}}}}ʙ.Clear();ˌ.GetItems
(ʙ);int ˣ=-1;MyFixedPoint ˤ=(MyFixedPoint)0;for(int ȱ=0;ȱ<ʙ.Count;ȱ++){var ƚ=ʙ[ȱ];if(!Ɇ(ƚ.Type)){continue;}if(!string.
Equals(ƚ.Type.SubtypeId,ˍ,ʄ.ʅ)){continue;}if(ƚ.Amount>ˤ){ˤ=ƚ.Amount;ˣ=ȱ;}}if(ˣ>0){ˌ.TransferItemTo(ˌ,ˣ,0,true,null);}}return
true;}public string Ü(bool ˬ){if(!ˬ||ʕ.Count==0||ʗ.Count==0){return"--- REFINERY CASCADE ---\n"+
"Refinery Balancing: OFF / STANDBY\n"+"(No scripted chunk pull; conveyors route ore when balancing is OFF.)";}int ȇ=ʕ.Count;int ø=ʗ.Count;int ˮ=0;int Ͱ=0;int
ͱ=0;for(int ó=0;ó<ȇ;ó++){int Ƹ=ʤ(ó,ȇ,ø);if(Ƹ==0){ˮ++;}else if(Ƹ==1){Ͱ++;}else{ͱ++;}}var Ͳ=new StringBuilder(320);Ͳ.
AppendLine("--- REFINERY CASCADE ---");Ͳ.Append("Total Refineries: ");Ͳ.AppendLine(ȇ.ToString());Ͳ.AppendLine(
"(Live chunk pull uses the top 3 ranked ores in order; deeper ranks are balanced separately.)");int ͳ=ø<3?ø:3;for(int ʹ=0;ʹ<ͳ;ʹ++){int Â=ʹ==0?ˮ:(ʹ==1?Ͱ:ͱ);Ͳ.Append('[');Ͳ.Append('P');Ͳ.Append((char)('1'+ʹ));Ͳ.
Append("] ");Ͳ.Append(ʗ[ʹ]);Ͳ.Append(": ");Ͳ.Append(Â);Ͳ.AppendLine(" assigned");}return Ͳ.ToString();}private static bool Ǭ()
{return E.p!=null&&E.p.Me!=null&&E.p.GridTerminalSystem!=null;}string Ƶ(){A Ͷ=E.q;return Ͷ!=null&&!string.IsNullOrEmpty(Ͷ
.õ)?Ͷ.õ:"[Manual]";}bool Ź(IMyTerminalBlock Ə){if(Ə==null){return false;}string Ʒ=Ƶ();if(string.IsNullOrEmpty(Ʒ)){return
false;}string ú=Ə.CustomName;return û.ü(ú,Ʒ);}private static bool ǻ(IMyTerminalBlock Ə){if(Ə==null||!Ə.IsSameConstructAs(E.p.
Me)){return false;}var Ƒ=Ə as IMyInventoryOwner;return Ƒ!=null&&Ƒ.InventoryCount>0;}void ʮ(List<IMyRefinery>ǩ){ʕ.Clear();ʖ
.Clear();if(ǩ==null){return;}for(int ó=0;ó<ǩ.Count;ó++){var ǣ=ǩ[ó];if(ǣ==null){continue;}var ͷ=ǣ.OutputInventory;if(ͷ!=
null){ʖ.Add(ͷ);}ʕ.Add(ǣ);}}void ʰ(bool ͺ){for(int ó=0;ó<ʕ.Count;ó++){var ý=ʕ[ó]as IMyProductionBlock;if(ý==null){continue;}ý
.UseConveyorSystem=!ͺ;}}private static bool Ɇ(MyItemType Ƹ){string ƹ=Ƹ.TypeId.ToString();return ƹ.IndexOf("Ore",
StringComparison.Ordinal)>=0;}private static bool ˆ(MyItemType Ƹ){return Ƹ.TypeId.ToString().IndexOf("Ingot",StringComparison.Ordinal)>=
0;}private static MyFixedPoint ˏ(IMyRefinery ł){var Ʈ=ł.InputInventory;if(Ʈ==null||Ʈ.MaxVolume.RawValue<=0){return(
MyFixedPoint)0;}return Ʈ.MaxVolume-Ʈ.CurrentVolume;}float ˎ(IMyRefinery ł,string ˊ){var Ʈ=ł.InputInventory;if(Ʈ==null){return 0f;}
float ƭ=0f;ʙ.Clear();Ʈ.GetItems(ʙ);for(int ȱ=0;ȱ<ʙ.Count;ȱ++){var ƚ=ʙ[ȱ];if(Ɇ(ƚ.Type)&&string.Equals(ƚ.Type.SubtypeId,ˊ,ʄ.ʅ))
{ƭ+=(float)ƚ.Amount;}}return ƭ;}bool Ά(IMyInventory ː,int ˑ,IMyInventory ͻ){if(ː==null||ͻ==null||ː==ͻ||ˑ<0){return false;
}ː.GetItems(ʙ);if(ˑ>=ʙ.Count){return false;}MyItemType ˡ=ʙ[ˑ].Type;if(!ː.CanTransferItemTo(ͻ,ˡ)){return false;}if(ː.
TransferItemTo(ͻ,ˑ,null,true,null)){return true;}ː.GetItems(ʙ);if(ˑ>=ʙ.Count){return false;}MyFixedPoint ƞ=ʙ[ˑ].Amount;if(ƞ<=(
MyFixedPoint)0){return false;}for(int ͼ=1;ͼ<=8;ͼ++){double Ɲ=(double)ƞ/(1<<ͼ);if(Ɲ<0.01){break;}MyFixedPoint ͽ=(MyFixedPoint)Ɲ;if(ͽ
<=(MyFixedPoint)0){continue;}if(ː.TransferItemTo(ͻ,ˑ,null,true,ͽ)){return true;}}return false;}bool ˠ(string ˊ,
IMyInventory Έ,out IMyInventory ː,out int ˑ){ː=null;ˑ=-1;MyFixedPoint ˤ=(MyFixedPoint)0;MyFixedPoint Ή=(MyFixedPoint)ʬ(ˊ,E.q);if(a==
null){return false;}for(int ɼ=0;ɼ<a.Count;ɼ++){var Ə=a[ɼ];if(Ź(Ə)){continue;}var Ƒ=Ə as IMyInventoryOwner;if(Ƒ==null){
continue;}var Ί=Ə as IMyRefinery;for(int ˀ=0;ˀ<Ƒ.InventoryCount;ˀ++){var Ʈ=Ƒ.GetInventory(ˀ);if(Ʈ==null||Ʈ==Έ||ʖ.Contains(Ʈ)){
continue;}ʙ.Clear();Ʈ.GetItems(ʙ);for(int ȱ=0;ȱ<ʙ.Count;ȱ++){var ƚ=ʙ[ȱ];if(!Ɇ(ƚ.Type)){continue;}if(!string.Equals(ƚ.Type.
SubtypeId,ˊ,ʄ.ʅ)){continue;}if(Ί!=null&&ƚ.Amount<=Ή){continue;}if(ƚ.Amount>ˤ){ˤ=ƚ.Amount;ː=Ʈ;ˑ=ȱ;}}}}return ː!=null&&ˑ>=0&&ˤ>(
MyFixedPoint)0;}bool ˋ(string ˊ,int Ό=-1){int Â=ʕ.Count;if(Â==0){return false;}const float Ύ=0.5f;int Ώ;float ΐ;if(Ό>=0&&Ό<Â){Ώ=Ό;if
(ˏ(ʕ[Ώ]).RawValue<=0){return false;}ΐ=ˎ(ʕ[Ώ],ˊ);}else{Ώ=-1;ΐ=float.PositiveInfinity;long Α=-1;for(int ó=0;ó<Â;ó++){
MyFixedPoint Β=ˏ(ʕ[ó]);if(Β.RawValue<=0){continue;}float ƞ=ˎ(ʕ[ó],ˊ);long Γ=Β.RawValue;if(ƞ<ΐ-1e-4f||(Math.Abs(ƞ-ΐ)<1e-4f&&Γ>Α)){ΐ=ƞ
;Α=Γ;Ώ=ó;}}if(Ώ<0){return false;}}IMyInventory ˌ=ʕ[Ώ].InputInventory;if(ˌ==null){return false;}int Δ=-1;float Ε=-1f;for(
int ó=0;ó<Â;ó++){if(ó==Ώ){continue;}float ƞ=ˎ(ʕ[ó],ˊ);if(ƞ>Ε){Ε=ƞ;Δ=ó;}}if(Δ>=0&&Ε>ΐ+Ύ){var Ζ=ʕ[Δ].InputInventory;if(Ζ!=
null&&Ζ!=ˌ){ʙ.Clear();Ζ.GetItems(ʙ);int Η=-1;MyFixedPoint Θ=(MyFixedPoint)0;for(int ȱ=0;ȱ<ʙ.Count;ȱ++){var ƚ=ʙ[ȱ];if(!Ɇ(ƚ.
Type)||!string.Equals(ƚ.Type.SubtypeId,ˊ,ʄ.ʅ)){continue;}if(ƚ.Amount>Θ){Θ=ƚ.Amount;Η=ȱ;}}if(Η>=0&&ˏ(ʕ[Ώ]).RawValue>0&&Ά(Ζ,Η,
ˌ)){return true;}}}IMyInventory ː;int ˑ;if(!ˠ(ˊ,ˌ,out ː,out ˑ)){return false;}return Ά(ː,ˑ,ˌ);}private static string ʨ(
string ʧ){if(string.IsNullOrEmpty(ʧ)){return null;}if(string.Equals(ʧ,"Ice",ʄ.ʅ)){return null;}return ʧ;}private static bool ʩ
(A ž,string Ι,out double Ƌ){Ƌ=0;if(ž==null||ž.ā==null||string.IsNullOrEmpty(Ι)){return false;}if(ž.ā.TryGetValue(Ι,out Ƌ)
){return true;}string Κ="Ingot/"+Ι;return ž.ā.TryGetValue(Κ,out Ƌ);}private static readonly string[]Λ={"Iron","Nickel",
"Silicon","Gravel"};double Π(A ž){double Μ=double.PositiveInfinity;int Ν=0;for(int ó=0;ó<Λ.Length;ó++){string Ξ=Λ[ó];double Ƌ;if(
!ʩ(ž,Ξ,out Ƌ)){continue;}Ν++;if(Ƌ<=0){if(1.0<Μ){Μ=1.0;}continue;}double Ο;if(!ʘ.TryGetValue(Ξ,out Ο)){Ο=0;}double ł=Ο/Ƌ;
if(ł<Μ){Μ=ł;}}if(Ν==0){return double.PositiveInfinity;}return Μ;}double Ρ(string Ι,A ž){double Ƌ;if(!ʩ(ž,Ι,out Ƌ)){return
double.PositiveInfinity;}if(Ƌ<=0){return 1.0;}double Ο;if(!ʘ.TryGetValue(Ι,out Ο)){Ο=0;}return Ο/Ƌ;}double Σ(string ˊ,A ž){if(
string.Equals(ˊ,"Stone",ʄ.ʅ)){return Π(ž);}string Ξ=ʨ(ˊ);if(Ξ==null){return double.PositiveInfinity;}return Ρ(Ξ,ž);}void ʳ(){A
ž=E.q;double Τ=ž!=null?ž.ĕ:0.05;if(Τ<0){Τ=0;}int Ĭ=ʗ.Count;for(int ȇ=0;ȇ<Ĭ-1;ȇ++){for(int ø=ȇ+1;ø<Ĭ;ø++){string Υ=ʗ[ȇ];
string Ͳ=ʗ[ø];double Φ=Σ(Υ,ž);double Χ=Σ(Ͳ,ž);if(Τ>0&&!string.IsNullOrEmpty(ʝ)){if(string.Equals(Υ,ʝ,ʄ.ʅ)){Φ-=Τ;}if(string.
Equals(Ͳ,ʝ,ʄ.ʅ)){Χ-=Τ;}}if(Φ>Χ||(Φ==Χ&&string.CompareOrdinal(Υ,Ͳ)>0)){ʗ[ȇ]=Ͳ;ʗ[ø]=Υ;}}}if(Ĭ>0){ʝ=ʗ[0];}else{ʝ=string.Empty;}}
Dictionary<string,int>ʴ(){ʚ.Clear();for(int ó=0;ó<ʗ.Count;ó++){ʚ[ʗ[ó]]=ó+1;}return ʚ;}private static string Ϊ(IMyRefinery ł,out
float Ψ){Ψ=0f;var Ʈ=ł.InputInventory;if(Ʈ==null){return string.Empty;}var ƙ=new List<MyInventoryItem>();Ʈ.GetItems(ƙ);for(int
ó=0;ó<ƙ.Count;ó++){var ƚ=ƙ[ó];if(Ɇ(ƚ.Type)){Ψ+=(float)ƚ.Amount;}}if(ƙ.Count==0){return string.Empty;}var Ω=ƙ[0];if(!Ɇ(Ω.
Type)){return string.Empty;}return Ω.Type.SubtypeId;}private static string έ(IMyRefinery ł,out float Ϋ){Ϋ=0f;var Ʈ=ł.
OutputInventory;if(Ʈ==null){return string.Empty;}string ά=string.Empty;float ˤ=0f;var ƙ=new List<MyInventoryItem>();Ʈ.GetItems(ƙ);for(
int ó=0;ó<ƙ.Count;ó++){var ƚ=ƙ[ó];if(!ˆ(ƚ.Type)){continue;}float ȇ=(float)ƚ.Amount;Ϋ+=ȇ;if(ȇ>ˤ){ˤ=ȇ;ά=ƚ.Type.SubtypeId;}}
return ά;}private static bool ή(IMyRefinery ł){var ý=ł as IMyProductionBlock;return ý!=null&&ý.IsProducing;}P ʸ(Dictionary<
string,int>ί){int Â=ʕ.Count;var á=new P();if(Â==0){á.ë=new string[0];á.ʺ=new string[0];á.ʻ=new float[0];á.ʼ=new string[0];á.ʽ=
new float[0];á.ò=new bool[0];á.ñ=new bool[0];ΰ(á,ί);return á;}var Ȯ=new string[Â];var α=new string[Â];var β=new float[Â];
var γ=new string[Â];var δ=new float[Â];var ε=new bool[Â];var ñ=new bool[Â];for(int ó=0;ó<Â;ó++){var Ί=ʕ[ó];Ȯ[ó]=Ȳ.ȳ(Ί.
CustomName);float ζ;string η=Ϊ(Ί,out ζ);α[ó]=Ȳ.ȳ(η);β[ó]=ζ;ñ[ó]=ζ>0.0001f;float θ;string ι=έ(Ί,out θ);γ[ó]=θ>0.0001f?Ȳ.ȳ(ι):string
.Empty;δ[ó]=θ;ε[ó]=ή(Ί);}á.ë=Ȯ;á.ʺ=α;á.ʻ=β;á.ʼ=γ;á.ʽ=δ;á.ò=ε;á.ñ=ñ;ΰ(á,ί);return á;}void ΰ(P á,Dictionary<string,int>ί){á
.ʾ=null;á.ʿ=null;if(ί==null||ί.Count==0){return;}ʛ.Clear();foreach(var Ō in ί){if(string.Equals(Ō.Key,"Ice",ʄ.ʅ)){
continue;}ʛ.Add(Ō);}ʛ.Sort((ȇ,ø)=>{int Ĭ=ȇ.Value.CompareTo(ø.Value);if(Ĭ!=0){return Ĭ;}return string.CompareOrdinal(ȇ.Key,ø.Key)
;});if(ʛ.Count==0){return;}int Â=ʛ.Count;int ƴ=(Â+1)/2;var Ͳ=new StringBuilder();for(int ó=0;ó<ƴ;ó++){if(ó>0){Ͳ.Append(
"  ");}Ͳ.Append(ó+1);Ͳ.Append(". ");Ͳ.Append(Ȳ.κ(ʛ[ó].Key));}á.ʾ=Ȳ.ȳ(Ͳ.ToString());Ͳ.Clear();for(int ó=ƴ;ó<Â;ó++){if(ó>ƴ){Ͳ.
Append("  ");}Ͳ.Append(ó+1);Ͳ.Append(". ");Ͳ.Append(Ȳ.κ(ʛ[ó].Key));}á.ʿ=Ȳ.ȳ(Ͳ.Length>0?Ͳ.ToString():string.Empty);}}public
class R{public float Ũ,ũ,Ū,ū,Ŭ,ŭ,Ů,ů;public int Ű,ű;public bool è;}public class X{public string[]Ƿ,ǹ;public float[]Ǹ;}public
class N{public float Ȗ,ȗ,Ș,ș,Ț,ț,Ȝ,ȝ,Ȟ,ȟ,Ƞ,ȡ,Ȣ,ȣ,Ȥ,ȥ,Ȧ,ȧ,Ȩ,ȩ,Ȫ,ȫ,Ȭ,ȭ,Ȑ,ê,Ú;}public class T{public float ɮ,ɯ,ɰ,ɱ,ɲ,ɳ,ɴ,ɵ,ɶ,ɷ;
public int ɸ,ɹ,ɺ;public bool Û;}public class P{public string[]ë,ʺ,ʼ;public float[]ʻ,ʽ;public bool[]ò,ñ;public string ʾ,ʿ;}
public class V{public bool è,Û,é,ì,í,ï,þ;public int ÿ;public string Ā;}public static class r{public const string z=
"SYS_STATUS",Ø="PB1_WARNINGS",Ò="PB1ToPB2_InventorySummary",Ó="PB1ToPB2_RefineryStatus",Ô="PB1ToPB2_IceStatus",Õ=
"PB1ToPB2_PowerStatus",Ö="PB1ToPB2_InventoryDynamic",s="PB2ToPB1";}public static class φ{private const string λ="1";public static string ã(
object á){if(á==null)return string.Empty;Type Ƹ=á.GetType();if(Ƹ==typeof(N))return μ((N)á);if(Ƹ==typeof(P))return ν((P)á);if(Ƹ
==typeof(R))return ξ((R)á);if(Ƹ==typeof(T))return ο((T)á);if(Ƹ==typeof(X))return π((X)á);if(Ƹ==typeof(V))return ρ((V)á);
return string.Empty;}private static string μ(N ŧ){StringBuilder Ͳ=new StringBuilder(512);Ͳ.Append(λ).Append(';');Ͳ.Append(ŧ.Ȗ)
.Append(';');Ͳ.Append(ŧ.ȗ).Append(';');Ͳ.Append(ŧ.Ș).Append(';');Ͳ.Append(ŧ.ș).Append(';');Ͳ.Append(ŧ.Ț).Append(';');Ͳ.
Append(ŧ.ț).Append(';');Ͳ.Append(ŧ.Ȝ).Append(';');Ͳ.Append(ŧ.ȝ).Append(';');Ͳ.Append(ŧ.Ȟ).Append(';');Ͳ.Append(ŧ.ȟ).Append(';'
);Ͳ.Append(ŧ.Ƞ).Append(';');Ͳ.Append(ŧ.ȡ).Append(';');Ͳ.Append(ŧ.Ȣ).Append(';');Ͳ.Append(ŧ.ȣ).Append(';');Ͳ.Append(ŧ.Ȥ).
Append(';');Ͳ.Append(ŧ.ȥ).Append(';');Ͳ.Append(ŧ.Ȧ).Append(';');Ͳ.Append(ŧ.ȧ).Append(';');Ͳ.Append(ŧ.Ȩ).Append(';');Ͳ.Append(ŧ
.ȩ).Append(';');Ͳ.Append(ŧ.Ȫ).Append(';');Ͳ.Append(ŧ.ȫ).Append(';');Ͳ.Append(ŧ.Ȭ).Append(';');Ͳ.Append(ŧ.ȭ).Append(';');Ͳ
.Append(ŧ.Ȑ).Append(';');Ͳ.Append(ŧ.ê).Append(';');Ͳ.Append(ŧ.Ú);return Ͳ.ToString();}private static string ν(P ŧ){
StringBuilder Ͳ=new StringBuilder(256);Ͳ.Append(λ).Append(';');Ͳ.Append(ς(ŧ.ë)).Append(';');Ͳ.Append(ς(ŧ.ʺ)).Append(';');Ͳ.Append(σ(ŧ
.ʻ)).Append(';');Ͳ.Append(ς(ŧ.ʼ)).Append(';');Ͳ.Append(σ(ŧ.ʽ)).Append(';');Ͳ.Append(τ(ŧ.ò)).Append(';');Ͳ.Append(τ(ŧ.ñ)).
Append(';');Ͳ.Append(ŧ.ʾ!=null?ŧ.ʾ:string.Empty).Append(';');Ͳ.Append(ŧ.ʿ!=null?ŧ.ʿ:string.Empty);return Ͳ.ToString();}private
static string ξ(R ŧ){StringBuilder Ͳ=new StringBuilder(128);Ͳ.Append(λ).Append(';');Ͳ.Append(ŧ.Ũ).Append(';');Ͳ.Append(ŧ.ũ).
Append(';');Ͳ.Append(ŧ.Ū).Append(';');Ͳ.Append(ŧ.ū).Append(';');Ͳ.Append(ŧ.Ŭ).Append(';');Ͳ.Append(ŧ.ŭ).Append(';');Ͳ.Append(ŧ
.Ů).Append(';');Ͳ.Append(ŧ.ů).Append(';');Ͳ.Append(ŧ.Ű).Append(';');Ͳ.Append(ŧ.ű).Append(';');Ͳ.Append(ŧ.è?'1':'0');
return Ͳ.ToString();}private static string ο(T ŧ){StringBuilder Ͳ=new StringBuilder(256);Ͳ.Append(λ).Append(';');Ͳ.Append(ŧ.ɮ)
.Append(';');Ͳ.Append(ŧ.ɯ).Append(';');Ͳ.Append(ŧ.ɰ).Append(';');Ͳ.Append(ŧ.ɱ).Append(';');Ͳ.Append(ŧ.ɲ).Append(';');Ͳ.
Append(ŧ.ɳ).Append(';');Ͳ.Append(ŧ.ɶ).Append(';');Ͳ.Append(ŧ.ɷ).Append(';');Ͳ.Append(ŧ.ɴ).Append(';');Ͳ.Append(ŧ.ɵ).Append(';'
);Ͳ.Append(ŧ.ɸ).Append(';');Ͳ.Append(ŧ.ɹ).Append(';');Ͳ.Append(ŧ.ɺ).Append(';');Ͳ.Append(ŧ.Û?'1':'0');return Ͳ.ToString()
;}private static string π(X ŧ){StringBuilder Ͳ=new StringBuilder(128);Ͳ.Append(λ).Append(';');Ͳ.Append(ς(ŧ.Ƿ)).Append(';'
);Ͳ.Append(σ(ŧ.Ǹ)).Append(';');Ͳ.Append(ς(ŧ.ǹ));return Ͳ.ToString();}private static string ρ(V ŧ){StringBuilder Ͳ=new
StringBuilder(128);Ͳ.Append(λ).Append(';');Ͳ.Append(ŧ.è?'1':'0').Append(';');Ͳ.Append(ŧ.Û?'1':'0').Append(';');Ͳ.Append(ŧ.é?'1':'0').
Append(';');Ͳ.Append(ŧ.ì?'1':'0').Append(';');Ͳ.Append(ŧ.í?'1':'0').Append(';');Ͳ.Append(ŧ.ï?'1':'0').Append(';');Ͳ.Append(ŧ.ÿ
).Append(';');Ͳ.Append(ŧ.Ā!=null?ŧ.Ā:string.Empty).Append(';');Ͳ.Append(ŧ.þ?'1':'0');return Ͳ.ToString();}private static
string ς(string[]ȇ){if(ȇ==null||ȇ.Length==0)return string.Empty;StringBuilder Ͳ=new StringBuilder(ȇ.Length*8);for(int ó=0;ó<ȇ.
Length;ó++){if(ó>0)Ͳ.Append('|');υ(Ͳ,ȇ[ó]);}return Ͳ.ToString();}private static string σ(float[]ȇ){if(ȇ==null||ȇ.Length==0)
return string.Empty;StringBuilder Ͳ=new StringBuilder(ȇ.Length*12);for(int ó=0;ó<ȇ.Length;ó++){if(ó>0)Ͳ.Append('|');Ͳ.Append(ȇ
[ó].ToString());}return Ͳ.ToString();}private static string τ(bool[]ȇ){if(ȇ==null||ȇ.Length==0)return string.Empty;
StringBuilder Ͳ=new StringBuilder(ȇ.Length*2);for(int ó=0;ó<ȇ.Length;ó++){if(ó>0)Ͳ.Append('|');Ͳ.Append(ȇ[ó]?'1':'0');}return Ͳ.
ToString();}private static void υ(StringBuilder Ͳ,string ʥ){if(ʥ==null)return;for(int ó=0;ó<ʥ.Length;ó++){char Ĭ=ʥ[ó];if(Ĭ=='\\'
){Ͳ.Append('\\');Ͳ.Append('\\');}else if(Ĭ=='|'){Ͳ.Append('\\');Ͳ.Append('|');}else Ͳ.Append(Ĭ);}}}public static class â{
public static string ã(object á){return φ.ã(á);}}public static class å{private const uint χ=2166136261u,ψ=16777619u;private
static long ω;public static uint ό(string ϊ){return ϋ(χ,ϊ);}public static string æ(string ύ,string ώ,string Ϗ){long ϐ=DateTime
.UtcNow.Ticks;if(ϐ<=ω){ϐ=ω+1;}ω=ϐ;string ͼ=ώ??"";string ϑ=ϐ.ToString();string ϒ=(ύ??"")+ϑ+ͼ+(Ϗ??"");uint ϓ=ό(ϒ);string ϔ=
ϓ.ToString("X8");string ϕ=ͼ.Length==0?"":Convert.ToBase64String(Encoding.UTF8.GetBytes(ͼ));return(ύ??"")+"|"+ϑ+"|"+ϕ+"|"+
ϔ;}private static uint ϋ(uint ϖ,string ʥ){if(ʥ==null||ʥ.Length==0){return ϖ;}for(int ó=0;ó<ʥ.Length;ó++){char Ĭ=ʥ[ó];ϖ^=(
byte)(Ĭ&0xFF);ϖ*=ψ;ϖ^=(byte)((Ĭ>>8)&0xFF);ϖ*=ψ;}return ϖ;}}public static class û{public static bool ü(string ú,string Ʒ){if(
string.IsNullOrEmpty(ú)||string.IsNullOrEmpty(Ʒ))return false;return ú.IndexOf(Ʒ,StringComparison.OrdinalIgnoreCase)>=0;}}
public static class Ȳ{private static readonly StringBuilder ϗ=new StringBuilder(48);public static string κ(string Ʌ){if(string
.IsNullOrEmpty(Ʌ)){return"-";}if(string.Equals(Ʌ,"Iron",ʄ.ʅ)){return"Fe";}if(string.Equals(Ʌ,"Nickel",ʄ.ʅ)){return"Ni";}
if(string.Equals(Ʌ,"Cobalt",ʄ.ʅ)){return"Co";}if(string.Equals(Ʌ,"Silicon",ʄ.ʅ)){return"Si";}if(string.Equals(Ʌ,"Silver",ʄ
.ʅ)){return"Ag";}if(string.Equals(Ʌ,"Gold",ʄ.ʅ)){return"Au";}if(string.Equals(Ʌ,"Magnesium",ʄ.ʅ)){return"Mg";}if(string.
Equals(Ʌ,"Platinum",ʄ.ʅ)){return"Pt";}if(string.Equals(Ʌ,"Uranium",ʄ.ʅ)){return"U";}if(string.Equals(Ʌ,"Stone",ʄ.ʅ)){return
"St";}if(string.Equals(Ʌ,"Ice",ʄ.ʅ)){return"Ic";}if(Ʌ.Length<=2){return Ʌ.ToUpperInvariant();}return Ʌ.Substring(0,2).
ToUpperInvariant();}public static string ȳ(string Ķ){if(string.IsNullOrEmpty(Ķ)){return string.Empty;}int Â=Ķ.Length;int Ϙ=-1;for(int ó=
0;ó<Â;ó++){char Ĭ=Ķ[ó];if(Ĭ==';'||Ĭ=='|'||Ĭ=='\\'||Ĭ=='\r'||Ĭ=='\n'){Ϙ=ó;break;}}if(Ϙ<0){return Ķ;}char[]ϙ=new char[Â];
for(int ó=0;ó<Ϙ;ó++){ϙ[ó]=Ķ[ó];}for(int ó=Ϙ;ó<Â;ó++){char Ĭ=Ķ[ó];ϙ[ó]=(Ĭ==';'||Ĭ=='|'||Ĭ=='\\'||Ĭ=='\r'||Ĭ=='\n')?' ':Ĭ;}
return new string(ϙ);}}public static class ƫ{public static float Ƭ(float Ő,float Ϛ,float ϛ){if(Ϛ>ϛ){float Ϝ=Ϛ;Ϛ=ϛ;ϛ=Ϝ;}if(Ő<Ϛ)
return Ϛ;if(Ő>ϛ)return ϛ;return Ő;}}public static class ʄ{public const StringComparison ʅ=StringComparison.OrdinalIgnoreCase;