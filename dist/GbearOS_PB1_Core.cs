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
[Â];var Ȱ=new string[Â];int ȱ=0;foreach(var Ō in ǥ){var ȕ=Ō.Value;Ȯ[ȱ]=ȕ.Ǩ;ȯ[ȱ]=(float)ȕ.Ǧ;Ȱ[ȱ]=ȕ.ǧ;ȱ++;}á.Ƿ=Ȯ;á.Ǹ=ȯ;á.ǹ=
Ȱ;}void ȅ(byte Ȳ,int ǿ){if(ǿ<=0||q==null||!q.Ĝ||p==null){return;}Ǚ.Clear();Ǚ.Append("INV ex=");Ǚ.Append(Ȳ);Ǚ.Append(" n="
);Ǚ.Append(ǿ);p.Echo(Ǚ.ToString());}bool Ȅ(IMyInventory ȳ,int ȴ){ǘ.Clear();ȳ.GetItems(ǘ);if(ȴ<0||ȴ>=ǘ.Count){return false
;}for(int ŧ=0;ŧ<Ǒ.Count;ŧ++){var Ų=Ǒ[ŧ];if(Ų==null||Ų==ȳ){continue;}if(ȵ(ȳ,ȴ,Ų)){return true;}}return false;}bool ȉ(
IMyInventory ȳ,int ȴ,IMyInventory ȶ=null){ǘ.Clear();ȳ.GetItems(ǘ);if(ȴ<0||ȴ>=ǘ.Count){return false;}MyItemType ȷ=ǘ[ȴ].Type;ȸ Ⱥ=ȹ(ȷ);
List<IMyInventory>Ȼ;switch(Ⱥ){case ȸ.ȼ:Ȼ=ǋ;break;case ȸ.Ƚ:Ȼ=ǌ;break;case ȸ.Ⱦ:Ȼ=Ǎ;break;case ȸ.ȿ:Ȼ=ǎ;break;case ȸ.ɀ:Ȼ=Ǐ;break
;case ȸ.Ɂ:Ȼ=ǐ;break;default:Ȼ=Ǒ;break;}for(int ŧ=0;ŧ<Ȼ.Count;ŧ++){var Ų=Ȼ[ŧ];if(Ų==null||Ų==ȳ||Ų==ȶ){continue;}if(ȵ(ȳ,ȴ,Ų
)){return true;}}if(Ⱥ==ȸ.ɂ){return false;}for(int ŧ=0;ŧ<Ǒ.Count;ŧ++){var Ų=Ǒ[ŧ];if(Ų==null||Ų==ȳ||Ų==ȶ){continue;}if(ȵ(ȳ,
ȴ,Ų)){return true;}}return false;}private static bool ȵ(IMyInventory ȳ,int ȴ,IMyInventory Ų){if(!ȳ.IsConnectedTo(Ų)){
return false;}return ȳ.TransferItemTo(Ų,ȴ,null,true,null);}void ȑ(IMyInventory Ʈ,ref N á){var ƙ=new List<MyInventoryItem>();Ʈ.
GetItems(ƙ);for(int ó=0;ó<ƙ.Count;ó++){var ƚ=ƙ[ó];var Ƹ=ƚ.Type;string Ƀ=Ƹ.SubtypeId;float ƞ=(float)ƚ.Amount;if(Ʉ(Ƹ)){if(ǂ.
Contains(Ƀ)){Ʌ(ref á,Ƀ,ƞ);}else if(ǔ.Add(Ƀ)){Ǖ.Add(Ƀ);}continue;}if(Ȇ(Ƹ)){if(Ǆ.Contains(Ƀ)){Ɇ(ref á,Ƀ,ƞ);}continue;}if(ȃ(Ƹ)){á.Ȫ
+=ƞ;continue;}if(ɇ(Ƹ)){á.ȫ+=ƞ;continue;}if(Ɉ(Ƹ)){á.Ȭ+=ƞ;continue;}if(ɉ(Ƹ)){á.ȭ+=ƞ;}}}private static void Ʌ(ref N á,string
Ƀ,float ƞ){switch(Ƀ.ToUpperInvariant()){case"IRON":á.Ȗ+=ƞ;break;case"NICKEL":á.ȗ+=ƞ;break;case"COBALT":á.Ș+=ƞ;break;case
"SILICON":á.ș+=ƞ;break;case"MAGNESIUM":á.Ț+=ƞ;break;case"SILVER":á.ț+=ƞ;break;case"GOLD":á.Ȝ+=ƞ;break;case"PLATINUM":á.ȝ+=ƞ;break
;case"URANIUM":á.Ȟ+=ƞ;break;case"STONE":á.ȟ+=ƞ;break;case"ICE":á.Ƞ+=ƞ;break;}}private static void Ɇ(ref N á,string Ƀ,
float ƞ){switch(Ƀ.ToUpperInvariant()){case"IRON":á.ȡ+=ƞ;break;case"NICKEL":á.Ȣ+=ƞ;break;case"COBALT":á.ȣ+=ƞ;break;case
"SILICON":á.Ȥ+=ƞ;break;case"MAGNESIUM":á.ȥ+=ƞ;break;case"SILVER":á.Ȧ+=ƞ;break;case"GOLD":á.ȧ+=ƞ;break;case"PLATINUM":á.Ȩ+=ƞ;break
;case"URANIUM":á.ȩ+=ƞ;break;}}bool Ȓ(MyItemType Ƹ){return true;}private static string ȓ(MyItemType Ƹ){if(Ʉ(Ƹ)){return
"Ore";}if(Ȇ(Ƹ)){return"Ingot";}if(ȃ(Ƹ)){return"Component";}if(ɇ(Ƹ)){return"Ammo";}if(Ɉ(Ƹ)){return"Tool";}if(ɉ(Ƹ)){return
"Bottle";}return"Other";}enum ȸ{ȼ,Ƚ,Ⱦ,ȿ,ɀ,Ɂ,ɂ}private static ȸ ȹ(MyItemType Ƹ){if(Ʉ(Ƹ)){return ȸ.ȼ;}if(Ȇ(Ƹ)){return ȸ.Ƚ;}if(ȃ(Ƹ)
){return ȸ.Ⱦ;}if(ɇ(Ƹ)){return ȸ.ȿ;}if(Ɉ(Ƹ)){return ȸ.ɀ;}if(ɉ(Ƹ)){return ȸ.Ɂ;}return ȸ.ɂ;}private static bool Ʉ(MyItemType
Ƹ){string ƹ=Ƹ.TypeId.ToString();return ƹ.IndexOf("Ore",StringComparison.Ordinal)>=0;}private static bool Ȇ(MyItemType Ƹ){
return Ƹ.TypeId.ToString().IndexOf("Ingot",StringComparison.Ordinal)>=0;}private static bool ȃ(MyItemType Ƹ){return Ƹ.TypeId.
ToString().IndexOf("Component",StringComparison.Ordinal)>=0;}private static bool ɇ(MyItemType Ƹ){string ƹ=Ƹ.TypeId.ToString();
return ƹ.IndexOf("Ammo",StringComparison.Ordinal)>=0||ƹ.IndexOf("Magazine",StringComparison.Ordinal)>=0;}private static bool Ɉ
(MyItemType Ƹ){string ƹ=Ƹ.TypeId.ToString();if(ƹ.IndexOf("PhysicalGun",StringComparison.Ordinal)>=0){return true;}if(ƹ.
IndexOf("Welder",StringComparison.Ordinal)>=0){return true;}if(ƹ.IndexOf("Drill",StringComparison.Ordinal)>=0&&ƹ.IndexOf(
"Component",StringComparison.Ordinal)<0){return true;}if(ƹ.IndexOf("Grinder",StringComparison.Ordinal)>=0){return true;}return
false;}private static bool ɉ(MyItemType Ƹ){string ƹ=Ƹ.TypeId.ToString();return ƹ.IndexOf("OxygenContainer",StringComparison.
Ordinal)>=0||ƹ.IndexOf("GasContainer",StringComparison.Ordinal)>=0;}bool Ǭ(){return p!=null&&p.Me!=null&&p.GridTerminalSystem!=
null;}private static bool ǟ(IMyTerminalBlock Ə){if(Ə==null){return false;}string Ʒ=q!=null&&!string.IsNullOrEmpty(q.õ)?q.õ:
"[Manual]";if(string.IsNullOrEmpty(Ʒ)){return false;}string ú=Ə.CustomName;return û.ü(ú,Ʒ);}bool Ź(IMyTerminalBlock Ə){return ǟ(Ə)
;}bool ǻ(IMyTerminalBlock Ə){if(Ə==null||!Ə.IsSameConstructAs(p.Me)){return false;}var Ƒ=Ə as IMyInventoryOwner;return Ƒ
!=null&&Ƒ.InventoryCount>0;}void Ǻ(IMyCargoContainer ƕ){string ú=ƕ.CustomName??string.Empty;bool Ɋ=û.ü(ú,ƻ);bool ɋ=û.ü(ú,Ƽ
);bool Ɍ=û.ü(ú,ƽ);bool ɍ=û.ü(ú,ƾ);bool Ɏ=û.ü(ú,ƿ);bool ɏ=û.ü(ú,ǀ);bool ɐ=û.ü(ú,ǁ);bool ɑ=Ɋ||ɋ||Ɍ||ɍ||Ɏ||ɏ;for(int Ɩ=0;Ɩ<ƕ
.InventoryCount;Ɩ++){var Ʈ=ƕ.GetInventory(Ɩ);if(Ɋ){ǋ.Add(Ʈ);}if(ɋ){ǌ.Add(Ʈ);}if(Ɍ){Ǎ.Add(Ʈ);}if(ɍ){ǎ.Add(Ʈ);}if(Ɏ){Ǐ.Add(
Ʈ);}if(ɏ){ǐ.Add(Ʈ);}if(ɐ||!ɑ){Ǒ.Add(Ʈ);}}}}public class K{private const double ɒ=1e-9;private readonly List<
IMyBatteryBlock>ɓ=new List<IMyBatteryBlock>();private readonly List<IMyReactor>ɔ=new List<IMyReactor>();private readonly List<
IMyPowerProducer>ɕ=new List<IMyPowerProducer>(),ɖ=new List<IMyPowerProducer>();double ɗ;double ɘ,ə,ɚ,ɛ,ɜ,ɝ,ɞ,ɟ,ɠ,ɡ;private const int ř=
38000;int Ś;byte ś=4;bool ɢ;public bool Í(List<IMyTerminalBlock>ŝ,List<IMyBatteryBlock>ɣ,ref T á){if(!Ǭ()){ś=4;ɤ(ref á);
return true;}if(ś==4){ɓ.Clear();ɔ.Clear();ɕ.Clear();ɖ.Clear();ɢ=false;ś=0;Ś=0;}if(ś==0){if(!ɥ(ŝ,ɣ)){return false;}ɦ();ɧ();ɨ();
var ɪ=ɩ();ɫ(á,ɪ);ś=4;return true;}return true;}private static void ɤ(ref T ŧ){ŧ.ɬ=0f;ŧ.ɭ=0f;ŧ.ɮ=0f;ŧ.ɯ=0f;ŧ.ɰ=0f;ŧ.ɱ=0f;ŧ.ɲ
=0f;ŧ.ɳ=0f;ŧ.ɴ=0f;ŧ.ɵ=0f;ŧ.ɶ=0;ŧ.ɷ=0;ŧ.ɸ=0;ŧ.Û=false;}private static void ɫ(T Ų,T ų){Ų.ɬ=ų.ɬ;Ų.ɭ=ų.ɭ;Ų.ɮ=ų.ɮ;Ų.ɯ=ų.ɯ;Ų.ɰ=
ų.ɰ;Ų.ɱ=ų.ɱ;Ų.ɲ=ų.ɲ;Ų.ɳ=ų.ɳ;Ų.ɴ=ų.ɴ;Ų.ɵ=ų.ɵ;Ų.ɶ=ų.ɶ;Ų.ɷ=ų.ɷ;Ų.ɸ=ų.ɸ;Ų.Û=ų.Û;}bool ɥ(List<IMyTerminalBlock>ŝ,List<
IMyBatteryBlock>ɣ){if(ŝ==null){return true;}var ɹ=E.p.Me;if(!ɢ&&ɣ!=null){for(int ɺ=0;ɺ<ɣ.Count;ɺ++){var ɻ=ɣ[ɺ];if(ɻ!=null&&ɻ.
IsSameConstructAs(ɹ)&&!Ź(ɻ)){ɓ.Add(ɻ);}}ɢ=true;}for(int ó=Ś;ó<ŝ.Count;ó++){if(E.p.Runtime.CurrentInstructionCount>ř){Ś=ó;return false;}
var Ə=ŝ[ó];if(!Ə.IsSameConstructAs(ɹ)||Ź(Ə)){continue;}if(Ə is IMyBatteryBlock){continue;}var ɼ=Ə as IMyReactor;if(ɼ!=null)
{ɔ.Add(ɼ);continue;}var ɽ=Ə as IMyPowerProducer;if(ɽ!=null){if(ɾ(ɽ)){ɕ.Add(ɽ);}else if(ɿ(ɽ)){ɖ.Add(ɽ);}}}return true;}
private static bool ɿ(IMyPowerProducer ø){var ʀ=ø as IMyTerminalBlock;if(ʀ==null){return false;}string ʁ=ʀ.BlockDefinition.
ToString();return ʁ.IndexOf("SolarPanel",ʂ.ʃ)>=0;}private static bool ɾ(IMyPowerProducer ø){var ʀ=ø as IMyTerminalBlock;if(ʀ==
null){return false;}string ʁ=ʀ.BlockDefinition.ToString();return ʁ.IndexOf("HydrogenEngine",ʂ.ʃ)>=0;}void ɦ(){ɗ=0;ɘ=0;ə=0;ɚ=
0;ɛ=0;ɜ=0;ɝ=0;ɞ=0;ɟ=0;ɠ=0;ɡ=0;for(int ó=0;ó<ɓ.Count;ó++){var ɻ=ɓ[ó];ɗ+=ɻ.CurrentStoredPower;ɘ+=ɻ.MaxStoredPower;ə+=ɻ.
CurrentInput;ɚ+=ɻ.CurrentOutput;ɞ+=ɻ.MaxInput;ɟ+=ɻ.MaxOutput;}for(int ó=0;ó<ɖ.Count;ó++){ɛ+=ɖ[ó].CurrentOutput;}for(int ó=0;ó<ɔ.
Count;ó++){ɜ+=ɔ[ó].CurrentOutput;ɠ+=ɔ[ó].MaxOutput;}for(int ó=0;ó<ɕ.Count;ó++){ɝ+=ɕ[ó].CurrentOutput;ɡ+=ɕ[ó].MaxOutput;}}void
ɧ(){if(ɓ.Count==0){return;}var ž=E.q;double ʄ=0.25;double ʅ=0.80;if(ž!=null){ʄ=ž.Ē;ʅ=ž.ē;}double ʆ=0;if(ɘ>ɒ){ʆ=ɗ/ɘ;}bool
ʇ=ʆ<ʄ;bool ʈ=ʆ>ʅ;for(int ó=0;ó<ɓ.Count;ó++){var ɻ=ɓ[ó];if(ʇ){ɻ.ChargeMode=ChargeMode.Recharge;}else if(ʈ){ɻ.ChargeMode=
ChargeMode.Auto;}}}void ɨ(){var ž=E.q;if(ž==null||!ž.Ė){return;}bool ʉ=ɛ<ž.Ĕ;bool ʊ=ɛ>=ž.Ĕ;if(ʉ){for(int ó=0;ó<ɔ.Count;ó++){ɔ[ó].
Enabled=true;}for(int ó=0;ó<ɕ.Count;ó++){ɕ[ó].Enabled=true;}}else if(ʊ){for(int ó=0;ó<ɔ.Count;ó++){ɔ[ó].Enabled=false;}for(int
ó=0;ó<ɕ.Count;ó++){ɕ[ó].Enabled=false;}}}T ɩ(){var á=new T();á.ɬ=(float)ɗ;á.ɭ=(float)ɘ;á.ɮ=(float)ə;á.ɯ=(float)ɚ;á.ɰ=(
float)ɞ;á.ɱ=(float)ɟ;á.ɲ=(float)ɜ;á.ɳ=(float)ɝ;á.ɴ=(float)ɠ;á.ɵ=(float)ɡ;á.ɶ=ɓ.Count;á.ɷ=ɔ.Count;á.ɸ=ɕ.Count;double ʋ=0;if(ɘ>
ɒ){ʋ=ɗ/ɘ;}double ʌ=0.25;if(E.q!=null){ʌ=E.q.Ē;}á.Û=ɘ>ɒ&&ʋ<ʌ;return á;}bool Ǭ(){return E.p!=null&&E.p.Me!=null&&E.p.
GridTerminalSystem!=null;}bool Ź(IMyTerminalBlock Ə){if(Ə==null){return false;}var ž=E.q;string Ʒ=ž!=null&&!string.IsNullOrEmpty(ž.õ)?ž.õ:
"[Manual]";if(string.IsNullOrEmpty(Ʒ)){return false;}string ú=Ə.CustomName;return û.ü(ú,Ʒ);}}public sealed class G{private const
double ʍ=0.05,ʎ=1000,ʏ=100000,ʐ=50000,ʑ=0.3,ʒ=125000;private readonly List<IMyRefinery>ʓ=new List<IMyRefinery>();private
readonly HashSet<IMyInventory>ʔ=new HashSet<IMyInventory>();List<IMyTerminalBlock>a;private readonly List<string>ʕ=new List<
string>();private readonly Dictionary<string,double>ʖ=new Dictionary<string,double>(StringComparer.OrdinalIgnoreCase);private
readonly List<MyInventoryItem>ʗ=new List<MyInventoryItem>();private readonly Dictionary<string,int>ʘ=new Dictionary<string,int>(
StringComparer.OrdinalIgnoreCase);private readonly List<KeyValuePair<string,int>>ʙ=new List<KeyValuePair<string,int>>(32);private
static readonly int[]ʚ={2000,1000,500,250,100,50};string ʛ=string.Empty;private const int ř=38000;int Ś,ʜ;byte ʝ=255;
Dictionary<string,int>ʞ;private static int ʢ(int ó,int ȇ,int ø){if(ø<=0){return 0;}if(ȇ<4){return 0;}int Ƹ;if(ȇ<8){int ʟ=(2*ȇ+2)/3
;Ƹ=ó<ʟ?0:1;}else{int ʟ=(ȇ+1)/2;int ʠ=ȇ-ʟ;int ʡ=(ʠ+1)/2;if(ó<ʟ){Ƹ=0;}else if(ó<ʟ+ʡ){Ƹ=1;}else{Ƹ=2;}}if(Ƹ>=ø){Ƹ=ø-1;}return
Ƹ;}private static double ʤ(string ʣ){if(ʣ==null){return ʑ;}switch(ʣ.ToLowerInvariant()){case"iron":case"silicon":return
0.7;case"nickel":return 0.4;case"cobalt":return 0.3;case"magnesium":return 0.007;case"silver":return 0.1;case"gold":return
0.01;case"platinum":return 0.005;case"uranium":return 0.01;default:return ʑ;}}private static double ʪ(string ʥ,A ž){if(
string.Equals(ʥ,"Stone",ʂ.ʃ)){return ʐ;}double ß;string ý=ʦ(ʥ);if(ý==null||!ʧ(ž,ý,out ß)||ß<=0){if(!ʧ(ž,"Iron",out ß)||ß<=0){ß
=ʒ;}}double ʨ=ʤ(ʥ);if(ʨ<=0){ʨ=ʑ;}double ʩ=ʍ*(ß/ʨ);if(ʩ<ʎ){ʩ=ʎ;}if(ʩ>ʏ){ʩ=ʏ;}return ʩ;}public bool Ê(List<IMyTerminalBlock
>ŝ,List<IMyRefinery>ǩ,ref P á){if(!Ǭ()){ʝ=255;ʫ(á);return true;}if(ʝ==255){a=ŝ;ʬ(ǩ);A ʭ=E.q;ʮ(ʭ!=null&&ʭ.Ý);ʕ.Clear();Ś=0
;ʝ=1;return false;}if(ʝ==1){if(!ʯ()){return false;}ʖ.Clear();Ś=0;ʝ=2;return false;}if(ʝ==2){if(!ʰ()){return false;}ʱ();ʞ=
ʲ();A ž=E.q;bool ʳ=ž!=null&&ž.Ý;if(ʳ){ʜ=0;Ś=0;ʝ=4;}else{ʝ=6;}return false;}if(ʝ==4){if(!ʴ()){return false;}Ś=0;ʝ=5;return
false;}if(ʝ==5){if(!ʵ()){return false;}ʝ=6;return false;}if(ʝ==6){var ɪ=ʶ(ʞ);ʷ(á,ɪ);ʝ=255;return true;}return true;}private
static void ʫ(P ŧ){ŧ.ë=new string[0];ŧ.ʸ=new string[0];ŧ.ʹ=new float[0];ŧ.ʺ=new string[0];ŧ.ʻ=new float[0];ŧ.ò=new bool[0];ŧ.ñ
=new bool[0];ŧ.ʼ=null;ŧ.ʽ=null;}private static void ʷ(P Ų,P ų){Ų.ë=ų.ë;Ų.ʸ=ų.ʸ;Ų.ʹ=ų.ʹ;Ų.ʺ=ų.ʺ;Ų.ʻ=ų.ʻ;Ų.ò=ų.ò;Ų.ñ=ų.ñ;Ų.
ʼ=ų.ʼ;Ų.ʽ=ų.ʽ;}bool ʯ(){if(a==null){return true;}for(int ɺ=Ś;ɺ<a.Count;ɺ++){if(E.p.Runtime.CurrentInstructionCount>ř){Ś=ɺ
;return false;}var Ə=a[ɺ];if(Ź(Ə)){continue;}var Ƒ=Ə as IMyInventoryOwner;if(Ƒ==null){continue;}for(int ʾ=0;ʾ<Ƒ.
InventoryCount;ʾ++){var Ʈ=Ƒ.GetInventory(ʾ);if(Ʈ==null){continue;}var ƙ=new List<MyInventoryItem>();Ʈ.GetItems(ƙ);for(int ȱ=0;ȱ<ƙ.
Count;ȱ++){var Ƹ=ƙ[ȱ].Type;if(!Ʉ(Ƹ)){continue;}string Ƀ=Ƹ.SubtypeId;if(string.Equals(Ƀ,"Ice",ʂ.ʃ)){continue;}bool ʿ=false;for
(int ń=0;ń<ʕ.Count;ń++){if(string.Equals(ʕ[ń],Ƀ,ʂ.ʃ)){ʿ=true;break;}}if(!ʿ){ʕ.Add(Ƀ);}}}}return true;}bool ʰ(){if(a==null
){return true;}for(int ɺ=Ś;ɺ<a.Count;ɺ++){if(E.p.Runtime.CurrentInstructionCount>ř){Ś=ɺ;return false;}var Ə=a[ɺ];if(Ź(Ə))
{continue;}var Ƒ=Ə as IMyInventoryOwner;if(Ƒ==null){continue;}for(int ʾ=0;ʾ<Ƒ.InventoryCount;ʾ++){var Ʈ=Ƒ.GetInventory(ʾ)
;if(Ʈ==null){continue;}var ƙ=new List<MyInventoryItem>();Ʈ.GetItems(ƙ);for(int ȱ=0;ȱ<ƙ.Count;ȱ++){var ƚ=ƙ[ȱ];if(!ˀ(ƚ.Type
)){continue;}string Ƀ=ƚ.Type.SubtypeId;double ƞ=(double)ƚ.Amount;double ˁ;if(!ʖ.TryGetValue(Ƀ,out ˁ)){ˁ=0;}ʖ[Ƀ]=ˁ+ƞ;}}}
return true;}bool ʴ(){if(ʕ.Count==0){return true;}int ˆ=0;const int ˇ=20;for(int Ƹ=ʜ;Ƹ<ʕ.Count&&ˆ<ˇ;Ƹ++){if(E.p.Runtime.
CurrentInstructionCount>ř){ʜ=Ƹ;return false;}string ˈ=ʕ[Ƹ];while(ˆ<ˇ&&ˉ(ˈ)){if(E.p.Runtime.CurrentInstructionCount>ř){ʜ=Ƹ;return false;}ˆ++;}}
return true;}bool ʵ(){if(ʕ.Count==0){return true;}int ȇ=ʓ.Count;int ø=ʕ.Count;for(int ó=Ś;ó<ȇ;ó++){if(E.p.Runtime.
CurrentInstructionCount>ř){Ś=ó;return false;}var ł=ʓ[ó];var ˊ=ł.InputInventory;if(ˊ==null){continue;}string ˋ=ʕ[ʢ(ó,ȇ,ø)];if(ˌ(ł,ˋ)<=50f&&ˍ(ł).
RawValue>0){IMyInventory ˎ;int ˏ;if(ː(ˋ,ˊ,out ˎ,out ˏ)){for(int ń=0;ń<ʚ.Length;ń++){if(E.p.Runtime.CurrentInstructionCount>ř){Ś=
ó;return false;}ˎ.GetItems(ʗ);if(ˏ>=ʗ.Count){break;}MyItemType ˑ=ʗ[ˏ].Type;if(!ˎ.CanTransferItemTo(ˊ,ˑ)){break;}
MyFixedPoint ƞ=ʗ[ˏ].Amount;if(ƞ<=(MyFixedPoint)0){break;}MyFixedPoint ˠ=(MyFixedPoint)ʚ[ń];if(ƞ<ˠ){ˠ=ƞ;}if(ˠ<=(MyFixedPoint)0){
continue;}if(ˎ.TransferItemTo(ˊ,ˏ,null,true,ˠ)){break;}}}}ʗ.Clear();ˊ.GetItems(ʗ);int ˡ=-1;MyFixedPoint ˢ=(MyFixedPoint)0;for(
int ȱ=0;ȱ<ʗ.Count;ȱ++){var ƚ=ʗ[ȱ];if(!Ʉ(ƚ.Type)){continue;}if(!string.Equals(ƚ.Type.SubtypeId,ˋ,ʂ.ʃ)){continue;}if(ƚ.Amount
>ˢ){ˢ=ƚ.Amount;ˡ=ȱ;}}if(ˡ>0){ˊ.TransferItemTo(ˊ,ˡ,0,true,null);}}return true;}public string Ü(bool ˣ){if(!ˣ||ʓ.Count==0||
ʕ.Count==0){return"--- REFINERY CASCADE ---\n"+"Refinery Balancing: OFF / STANDBY\n"+
"(No scripted chunk pull; conveyors route ore when balancing is OFF.)";}int ȇ=ʓ.Count;int ø=ʕ.Count;int ˤ=0;int ˬ=0;int ˮ=0;for(int ó=0;ó<ȇ;ó++){int Ƹ=ʢ(ó,ȇ,ø);if(Ƹ==0){ˤ++;}else if(Ƹ==1){ˬ
++;}else{ˮ++;}}var Ͱ=new StringBuilder(320);Ͱ.AppendLine("--- REFINERY CASCADE ---");Ͱ.Append("Total Refineries: ");Ͱ.
AppendLine(ȇ.ToString());Ͱ.AppendLine(
"(Live chunk pull uses the top 3 ranked ores in order; deeper ranks are balanced separately.)");int ͱ=ø<3?ø:3;for(int Ͳ=0;Ͳ<ͱ;Ͳ++){int Â=Ͳ==0?ˤ:(Ͳ==1?ˬ:ˮ);Ͱ.Append('[');Ͱ.Append('P');Ͱ.Append((char)('1'+Ͳ));Ͱ.
Append("] ");Ͱ.Append(ʕ[Ͳ]);Ͱ.Append(": ");Ͱ.Append(Â);Ͱ.AppendLine(" assigned");}return Ͱ.ToString();}private static bool Ǭ()
{return E.p!=null&&E.p.Me!=null&&E.p.GridTerminalSystem!=null;}string Ƶ(){A ͳ=E.q;return ͳ!=null&&!string.IsNullOrEmpty(ͳ
.õ)?ͳ.õ:"[Manual]";}bool Ź(IMyTerminalBlock Ə){if(Ə==null){return false;}string Ʒ=Ƶ();if(string.IsNullOrEmpty(Ʒ)){return
false;}string ú=Ə.CustomName;return û.ü(ú,Ʒ);}private static bool ǻ(IMyTerminalBlock Ə){if(Ə==null||!Ə.IsSameConstructAs(E.p.
Me)){return false;}var Ƒ=Ə as IMyInventoryOwner;return Ƒ!=null&&Ƒ.InventoryCount>0;}void ʬ(List<IMyRefinery>ǩ){ʓ.Clear();ʔ
.Clear();if(ǩ==null){return;}for(int ó=0;ó<ǩ.Count;ó++){var ǣ=ǩ[ó];if(ǣ==null){continue;}var ʹ=ǣ.OutputInventory;if(ʹ!=
null){ʔ.Add(ʹ);}ʓ.Add(ǣ);}}void ʮ(bool Ͷ){for(int ó=0;ó<ʓ.Count;ó++){var ý=ʓ[ó]as IMyProductionBlock;if(ý==null){continue;}ý
.UseConveyorSystem=!Ͷ;}}private static bool Ʉ(MyItemType Ƹ){string ƹ=Ƹ.TypeId.ToString();return ƹ.IndexOf("Ore",
StringComparison.Ordinal)>=0;}private static bool ˀ(MyItemType Ƹ){return Ƹ.TypeId.ToString().IndexOf("Ingot",StringComparison.Ordinal)>=
0;}private static MyFixedPoint ˍ(IMyRefinery ł){var Ʈ=ł.InputInventory;if(Ʈ==null||Ʈ.MaxVolume.RawValue<=0){return(
MyFixedPoint)0;}return Ʈ.MaxVolume-Ʈ.CurrentVolume;}float ˌ(IMyRefinery ł,string ˈ){var Ʈ=ł.InputInventory;if(Ʈ==null){return 0f;}
float ƭ=0f;ʗ.Clear();Ʈ.GetItems(ʗ);for(int ȱ=0;ȱ<ʗ.Count;ȱ++){var ƚ=ʗ[ȱ];if(Ʉ(ƚ.Type)&&string.Equals(ƚ.Type.SubtypeId,ˈ,ʂ.ʃ))
{ƭ+=(float)ƚ.Amount;}}return ƭ;}bool ͼ(IMyInventory ˎ,int ˏ,IMyInventory ͷ){if(ˎ==null||ͷ==null||ˎ==ͷ||ˏ<0){return false;
}ˎ.GetItems(ʗ);if(ˏ>=ʗ.Count){return false;}MyItemType ˑ=ʗ[ˏ].Type;if(!ˎ.CanTransferItemTo(ͷ,ˑ)){return false;}if(ˎ.
TransferItemTo(ͷ,ˏ,null,true,null)){return true;}ˎ.GetItems(ʗ);if(ˏ>=ʗ.Count){return false;}MyFixedPoint ƞ=ʗ[ˏ].Amount;if(ƞ<=(
MyFixedPoint)0){return false;}for(int ͺ=1;ͺ<=8;ͺ++){double Ɲ=(double)ƞ/(1<<ͺ);if(Ɲ<0.01){break;}MyFixedPoint ͻ=(MyFixedPoint)Ɲ;if(ͻ
<=(MyFixedPoint)0){continue;}if(ˎ.TransferItemTo(ͷ,ˏ,null,true,ͻ)){return true;}}return false;}bool ː(string ˈ,
IMyInventory ͽ,out IMyInventory ˎ,out int ˏ){ˎ=null;ˏ=-1;MyFixedPoint ˢ=(MyFixedPoint)0;MyFixedPoint Ά=(MyFixedPoint)ʪ(ˈ,E.q);if(a==
null){return false;}for(int ɺ=0;ɺ<a.Count;ɺ++){var Ə=a[ɺ];if(Ź(Ə)){continue;}var Ƒ=Ə as IMyInventoryOwner;if(Ƒ==null){
continue;}var Έ=Ə as IMyRefinery;for(int ʾ=0;ʾ<Ƒ.InventoryCount;ʾ++){var Ʈ=Ƒ.GetInventory(ʾ);if(Ʈ==null||Ʈ==ͽ||ʔ.Contains(Ʈ)){
continue;}ʗ.Clear();Ʈ.GetItems(ʗ);for(int ȱ=0;ȱ<ʗ.Count;ȱ++){var ƚ=ʗ[ȱ];if(!Ʉ(ƚ.Type)){continue;}if(!string.Equals(ƚ.Type.
SubtypeId,ˈ,ʂ.ʃ)){continue;}if(Έ!=null&&ƚ.Amount<=Ά){continue;}if(ƚ.Amount>ˢ){ˢ=ƚ.Amount;ˎ=Ʈ;ˏ=ȱ;}}}}return ˎ!=null&&ˏ>=0&&ˢ>(
MyFixedPoint)0;}bool ˉ(string ˈ,int Ή=-1){int Â=ʓ.Count;if(Â==0){return false;}const float Ί=0.5f;int Ό;float Ύ;if(Ή>=0&&Ή<Â){Ό=Ή;if
(ˍ(ʓ[Ό]).RawValue<=0){return false;}Ύ=ˌ(ʓ[Ό],ˈ);}else{Ό=-1;Ύ=float.PositiveInfinity;long Ώ=-1;for(int ó=0;ó<Â;ó++){
MyFixedPoint ΐ=ˍ(ʓ[ó]);if(ΐ.RawValue<=0){continue;}float ƞ=ˌ(ʓ[ó],ˈ);long Α=ΐ.RawValue;if(ƞ<Ύ-1e-4f||(Math.Abs(ƞ-Ύ)<1e-4f&&Α>Ώ)){Ύ=ƞ
;Ώ=Α;Ό=ó;}}if(Ό<0){return false;}}IMyInventory ˊ=ʓ[Ό].InputInventory;if(ˊ==null){return false;}int Β=-1;float Γ=-1f;for(
int ó=0;ó<Â;ó++){if(ó==Ό){continue;}float ƞ=ˌ(ʓ[ó],ˈ);if(ƞ>Γ){Γ=ƞ;Β=ó;}}if(Β>=0&&Γ>Ύ+Ί){var Δ=ʓ[Β].InputInventory;if(Δ!=
null&&Δ!=ˊ){ʗ.Clear();Δ.GetItems(ʗ);int Ε=-1;MyFixedPoint Ζ=(MyFixedPoint)0;for(int ȱ=0;ȱ<ʗ.Count;ȱ++){var ƚ=ʗ[ȱ];if(!Ʉ(ƚ.
Type)||!string.Equals(ƚ.Type.SubtypeId,ˈ,ʂ.ʃ)){continue;}if(ƚ.Amount>Ζ){Ζ=ƚ.Amount;Ε=ȱ;}}if(Ε>=0&&ˍ(ʓ[Ό]).RawValue>0&&ͼ(Δ,Ε,
ˊ)){return true;}}}IMyInventory ˎ;int ˏ;if(!ː(ˈ,ˊ,out ˎ,out ˏ)){return false;}return ͼ(ˎ,ˏ,ˊ);}private static string ʦ(
string ʥ){if(string.IsNullOrEmpty(ʥ)){return null;}if(string.Equals(ʥ,"Ice",ʂ.ʃ)){return null;}return ʥ;}private static bool ʧ
(A ž,string Η,out double Ƌ){Ƌ=0;if(ž==null||ž.ā==null||string.IsNullOrEmpty(Η)){return false;}if(ž.ā.TryGetValue(Η,out Ƌ)
){return true;}string Θ="Ingot/"+Η;return ž.ā.TryGetValue(Θ,out Ƌ);}private static readonly string[]Ι={"Iron","Nickel",
"Silicon","Gravel"};double Ξ(A ž){double Κ=double.PositiveInfinity;int Λ=0;for(int ó=0;ó<Ι.Length;ó++){string Μ=Ι[ó];double Ƌ;if(
!ʧ(ž,Μ,out Ƌ)){continue;}Λ++;if(Ƌ<=0){if(1.0<Κ){Κ=1.0;}continue;}double Ν;if(!ʖ.TryGetValue(Μ,out Ν)){Ν=0;}double ł=Ν/Ƌ;
if(ł<Κ){Κ=ł;}}if(Λ==0){return double.PositiveInfinity;}return Κ;}double Ο(string Η,A ž){double Ƌ;if(!ʧ(ž,Η,out Ƌ)){return
double.PositiveInfinity;}if(Ƌ<=0){return 1.0;}double Ν;if(!ʖ.TryGetValue(Η,out Ν)){Ν=0;}return Ν/Ƌ;}double Π(string ˈ,A ž){if(
string.Equals(ˈ,"Stone",ʂ.ʃ)){return Ξ(ž);}string Μ=ʦ(ˈ);if(Μ==null){return double.PositiveInfinity;}return Ο(Μ,ž);}void ʱ(){A
ž=E.q;double Ρ=ž!=null?ž.ĕ:0.05;if(Ρ<0){Ρ=0;}int Ĭ=ʕ.Count;for(int ȇ=0;ȇ<Ĭ-1;ȇ++){for(int ø=ȇ+1;ø<Ĭ;ø++){string Σ=ʕ[ȇ];
string Ͱ=ʕ[ø];double Τ=Π(Σ,ž);double Υ=Π(Ͱ,ž);if(Ρ>0&&!string.IsNullOrEmpty(ʛ)){if(string.Equals(Σ,ʛ,ʂ.ʃ)){Τ-=Ρ;}if(string.
Equals(Ͱ,ʛ,ʂ.ʃ)){Υ-=Ρ;}}if(Τ>Υ||(Τ==Υ&&string.CompareOrdinal(Σ,Ͱ)>0)){ʕ[ȇ]=Ͱ;ʕ[ø]=Σ;}}}if(Ĭ>0){ʛ=ʕ[0];}else{ʛ=string.Empty;}}
Dictionary<string,int>ʲ(){ʘ.Clear();for(int ó=0;ó<ʕ.Count;ó++){ʘ[ʕ[ó]]=ó+1;}return ʘ;}private static string Ψ(IMyRefinery ł,out
float Φ){Φ=0f;var Ʈ=ł.InputInventory;if(Ʈ==null){return string.Empty;}var ƙ=new List<MyInventoryItem>();Ʈ.GetItems(ƙ);for(int
ó=0;ó<ƙ.Count;ó++){var ƚ=ƙ[ó];if(Ʉ(ƚ.Type)){Φ+=(float)ƚ.Amount;}}if(ƙ.Count==0){return string.Empty;}var Χ=ƙ[0];if(!Ʉ(Χ.
Type)){return string.Empty;}return Χ.Type.SubtypeId;}private static string Ϋ(IMyRefinery ł,out float Ω){Ω=0f;var Ʈ=ł.
OutputInventory;if(Ʈ==null){return string.Empty;}string Ϊ=string.Empty;float ˢ=0f;var ƙ=new List<MyInventoryItem>();Ʈ.GetItems(ƙ);for(
int ó=0;ó<ƙ.Count;ó++){var ƚ=ƙ[ó];if(!ˀ(ƚ.Type)){continue;}float ȇ=(float)ƚ.Amount;Ω+=ȇ;if(ȇ>ˢ){ˢ=ȇ;Ϊ=ƚ.Type.SubtypeId;}}
return Ϊ;}private static bool ά(IMyRefinery ł){var ý=ł as IMyProductionBlock;return ý!=null&&ý.IsProducing;}P ʶ(Dictionary<
string,int>έ){int Â=ʓ.Count;var á=new P();if(Â==0){á.ë=new string[0];á.ʸ=new string[0];á.ʹ=new float[0];á.ʺ=new string[0];á.ʻ=
new float[0];á.ò=new bool[0];á.ñ=new bool[0];ή(á,έ);return á;}var Ȯ=new string[Â];var ί=new string[Â];var ΰ=new float[Â];
var α=new string[Â];var β=new float[Â];var γ=new bool[Â];var ñ=new bool[Â];for(int ó=0;ó<Â;ó++){var Έ=ʓ[ó];Ȯ[ó]=δ(Έ.
CustomName);float ε;string ζ=Ψ(Έ,out ε);ί[ó]=ζ;ΰ[ó]=ε;ñ[ó]=ε>0.0001f;float η;string θ=Ϋ(Έ,out η);α[ó]=η>0.0001f?θ:string.Empty;β[ó
]=η;γ[ó]=ά(Έ);}á.ë=Ȯ;á.ʸ=ί;á.ʹ=ΰ;á.ʺ=α;á.ʻ=β;á.ò=γ;á.ñ=ñ;ή(á,έ);return á;}private static string δ(string Ķ){if(string.
IsNullOrEmpty(Ķ)){return string.Empty;}int Â=Ķ.Length;int ι=-1;for(int ó=0;ó<Â;ó++){char Ĭ=Ķ[ó];if(Ĭ==';'||Ĭ=='|'||Ĭ=='\\'||Ĭ=='\r'||
Ĭ=='\n'){ι=ó;break;}}if(ι<0){return Ķ;}char[]κ=new char[Â];for(int ó=0;ó<ι;ó++){κ[ó]=Ķ[ó];}for(int ó=ι;ó<Â;ó++){char Ĭ=Ķ[
ó];κ[ó]=(Ĭ==';'||Ĭ=='|'||Ĭ=='\\'||Ĭ=='\r'||Ĭ=='\n')?' ':Ĭ;}return new string(κ);}void ή(P á,Dictionary<string,int>έ){á.ʼ=
null;á.ʽ=null;if(έ==null||έ.Count==0){return;}ʙ.Clear();foreach(var Ō in έ){if(string.Equals(Ō.Key,"Ice",ʂ.ʃ)){continue;}ʙ.
Add(Ō);}ʙ.Sort((ȇ,ø)=>{int Ĭ=ȇ.Value.CompareTo(ø.Value);if(Ĭ!=0){return Ĭ;}return string.CompareOrdinal(ȇ.Key,ø.Key);});if(
ʙ.Count==0){return;}int Â=ʙ.Count;int ƴ=(Â+1)/2;var Ͱ=new StringBuilder();for(int ó=0;ó<ƴ;ó++){if(ó>0){Ͱ.Append("  ");}Ͱ.
Append(ó+1);Ͱ.Append(". ");Ͱ.Append(λ.μ(ʙ[ó].Key));}á.ʼ=Ͱ.ToString();Ͱ.Clear();for(int ó=ƴ;ó<Â;ó++){if(ó>ƴ){Ͱ.Append("  ");}Ͱ.
Append(ó+1);Ͱ.Append(". ");Ͱ.Append(λ.μ(ʙ[ó].Key));}á.ʽ=Ͱ.Length>0?Ͱ.ToString():string.Empty;}}public class R{public float Ũ,ũ
,Ū,ū,Ŭ,ŭ,Ů,ů;public int Ű,ű;public bool è;}public class X{public string[]Ƿ,ǹ;public float[]Ǹ;}public class N{public float
Ȗ,ȗ,Ș,ș,Ț,ț,Ȝ,ȝ,Ȟ,ȟ,Ƞ,ȡ,Ȣ,ȣ,Ȥ,ȥ,Ȧ,ȧ,Ȩ,ȩ,Ȫ,ȫ,Ȭ,ȭ,Ȑ,ê,Ú;}public class T{public float ɬ,ɭ,ɮ,ɯ,ɰ,ɱ,ɲ,ɳ,ɴ,ɵ;public int ɶ,ɷ,ɸ;
public bool Û;}public class P{public string[]ë,ʸ,ʺ;public float[]ʹ,ʻ;public bool[]ò,ñ;public string ʼ,ʽ;}public class V{public
bool è,Û,é,ì,í,ï,þ;public int ÿ;public string Ā;}public static class r{public const string z="SYS_STATUS",Ø="PB1_WARNINGS",Ò
="PB1ToPB2_InventorySummary",Ó="PB1ToPB2_RefineryStatus",Ô="PB1ToPB2_IceStatus",Õ="PB1ToPB2_PowerStatus",Ö=
"PB1ToPB2_InventoryDynamic",s="PB2ToPB1";}public static class ψ{private const string ν="1";public static string ã(object á){if(á==null)return
string.Empty;Type Ƹ=á.GetType();if(Ƹ==typeof(N))return ξ((N)á);if(Ƹ==typeof(P))return ο((P)á);if(Ƹ==typeof(R))return π((R)á);
if(Ƹ==typeof(T))return ρ((T)á);if(Ƹ==typeof(X))return ς((X)á);if(Ƹ==typeof(V))return σ((V)á);return string.Empty;}private
static string ξ(N ŧ){StringBuilder Ͱ=new StringBuilder(512);Ͱ.Append(ν).Append(';');Ͱ.Append(ŧ.Ȗ).Append(';');Ͱ.Append(ŧ.ȗ).
Append(';');Ͱ.Append(ŧ.Ș).Append(';');Ͱ.Append(ŧ.ș).Append(';');Ͱ.Append(ŧ.Ț).Append(';');Ͱ.Append(ŧ.ț).Append(';');Ͱ.Append(ŧ
.Ȝ).Append(';');Ͱ.Append(ŧ.ȝ).Append(';');Ͱ.Append(ŧ.Ȟ).Append(';');Ͱ.Append(ŧ.ȟ).Append(';');Ͱ.Append(ŧ.Ƞ).Append(';');Ͱ
.Append(ŧ.ȡ).Append(';');Ͱ.Append(ŧ.Ȣ).Append(';');Ͱ.Append(ŧ.ȣ).Append(';');Ͱ.Append(ŧ.Ȥ).Append(';');Ͱ.Append(ŧ.ȥ).
Append(';');Ͱ.Append(ŧ.Ȧ).Append(';');Ͱ.Append(ŧ.ȧ).Append(';');Ͱ.Append(ŧ.Ȩ).Append(';');Ͱ.Append(ŧ.ȩ).Append(';');Ͱ.Append(ŧ
.Ȫ).Append(';');Ͱ.Append(ŧ.ȫ).Append(';');Ͱ.Append(ŧ.Ȭ).Append(';');Ͱ.Append(ŧ.ȭ).Append(';');Ͱ.Append(ŧ.Ȑ).Append(';');Ͱ
.Append(ŧ.ê).Append(';');Ͱ.Append(ŧ.Ú);return Ͱ.ToString();}private static string ο(P ŧ){StringBuilder Ͱ=new
StringBuilder(256);Ͱ.Append(ν).Append(';');Ͱ.Append(τ(ŧ.ë)).Append(';');Ͱ.Append(τ(ŧ.ʸ)).Append(';');Ͱ.Append(υ(ŧ.ʹ)).Append(';');Ͱ.
Append(τ(ŧ.ʺ)).Append(';');Ͱ.Append(υ(ŧ.ʻ)).Append(';');Ͱ.Append(φ(ŧ.ò)).Append(';');Ͱ.Append(φ(ŧ.ñ)).Append(';');Ͱ.Append(ŧ.ʼ
!=null?ŧ.ʼ:string.Empty).Append(';');Ͱ.Append(ŧ.ʽ!=null?ŧ.ʽ:string.Empty);return Ͱ.ToString();}private static string π(R ŧ
){StringBuilder Ͱ=new StringBuilder(128);Ͱ.Append(ν).Append(';');Ͱ.Append(ŧ.Ũ).Append(';');Ͱ.Append(ŧ.ũ).Append(';');Ͱ.
Append(ŧ.Ū).Append(';');Ͱ.Append(ŧ.ū).Append(';');Ͱ.Append(ŧ.Ŭ).Append(';');Ͱ.Append(ŧ.ŭ).Append(';');Ͱ.Append(ŧ.Ů).Append(';'
);Ͱ.Append(ŧ.ů).Append(';');Ͱ.Append(ŧ.Ű).Append(';');Ͱ.Append(ŧ.ű).Append(';');Ͱ.Append(ŧ.è?'1':'0');return Ͱ.ToString()
;}private static string ρ(T ŧ){StringBuilder Ͱ=new StringBuilder(256);Ͱ.Append(ν).Append(';');Ͱ.Append(ŧ.ɬ).Append(';');Ͱ
.Append(ŧ.ɭ).Append(';');Ͱ.Append(ŧ.ɮ).Append(';');Ͱ.Append(ŧ.ɯ).Append(';');Ͱ.Append(ŧ.ɰ).Append(';');Ͱ.Append(ŧ.ɱ).
Append(';');Ͱ.Append(ŧ.ɴ).Append(';');Ͱ.Append(ŧ.ɵ).Append(';');Ͱ.Append(ŧ.ɲ).Append(';');Ͱ.Append(ŧ.ɳ).Append(';');Ͱ.Append(ŧ
.ɶ).Append(';');Ͱ.Append(ŧ.ɷ).Append(';');Ͱ.Append(ŧ.ɸ).Append(';');Ͱ.Append(ŧ.Û?'1':'0');return Ͱ.ToString();}private
static string ς(X ŧ){StringBuilder Ͱ=new StringBuilder(128);Ͱ.Append(ν).Append(';');Ͱ.Append(τ(ŧ.Ƿ)).Append(';');Ͱ.Append(υ(ŧ.
Ǹ)).Append(';');Ͱ.Append(τ(ŧ.ǹ));return Ͱ.ToString();}private static string σ(V ŧ){StringBuilder Ͱ=new StringBuilder(128)
;Ͱ.Append(ν).Append(';');Ͱ.Append(ŧ.è?'1':'0').Append(';');Ͱ.Append(ŧ.Û?'1':'0').Append(';');Ͱ.Append(ŧ.é?'1':'0').Append
(';');Ͱ.Append(ŧ.ì?'1':'0').Append(';');Ͱ.Append(ŧ.í?'1':'0').Append(';');Ͱ.Append(ŧ.ï?'1':'0').Append(';');Ͱ.Append(ŧ.ÿ)
.Append(';');Ͱ.Append(ŧ.Ā!=null?ŧ.Ā:string.Empty).Append(';');Ͱ.Append(ŧ.þ?'1':'0');return Ͱ.ToString();}private static
string τ(string[]ȇ){if(ȇ==null||ȇ.Length==0)return string.Empty;StringBuilder Ͱ=new StringBuilder(ȇ.Length*8);for(int ó=0;ó<ȇ.
Length;ó++){if(ó>0)Ͱ.Append('|');χ(Ͱ,ȇ[ó]);}return Ͱ.ToString();}private static string υ(float[]ȇ){if(ȇ==null||ȇ.Length==0)
return string.Empty;StringBuilder Ͱ=new StringBuilder(ȇ.Length*12);for(int ó=0;ó<ȇ.Length;ó++){if(ó>0)Ͱ.Append('|');Ͱ.Append(ȇ
[ó].ToString());}return Ͱ.ToString();}private static string φ(bool[]ȇ){if(ȇ==null||ȇ.Length==0)return string.Empty;
StringBuilder Ͱ=new StringBuilder(ȇ.Length*2);for(int ó=0;ó<ȇ.Length;ó++){if(ó>0)Ͱ.Append('|');Ͱ.Append(ȇ[ó]?'1':'0');}return Ͱ.
ToString();}private static void χ(StringBuilder Ͱ,string ʣ){if(ʣ==null)return;for(int ó=0;ó<ʣ.Length;ó++){char Ĭ=ʣ[ó];if(Ĭ=='\\'
){Ͱ.Append('\\');Ͱ.Append('\\');}else if(Ĭ=='|'){Ͱ.Append('\\');Ͱ.Append('|');}else Ͱ.Append(Ĭ);}}}public static class â{
public static string ã(object á){return ψ.ã(á);}}public static class å{private const uint ω=2166136261u,ϊ=16777619u;private
static long ϋ;public static uint ώ(string ό){return ύ(ω,ό);}public static string æ(string Ϗ,string ϐ,string ϑ){long ϒ=DateTime
.UtcNow.Ticks;if(ϒ<=ϋ){ϒ=ϋ+1;}ϋ=ϒ;string ͺ=ϐ??"";string ϓ=ϒ.ToString();string ϔ=(Ϗ??"")+ϓ+ͺ+(ϑ??"");uint ϕ=ώ(ϔ);string ϖ=
ϕ.ToString("X8");string ϗ=ͺ.Length==0?"":Convert.ToBase64String(Encoding.UTF8.GetBytes(ͺ));return(Ϗ??"")+"|"+ϓ+"|"+ϗ+"|"+
ϖ;}private static uint ύ(uint Ϙ,string ʣ){if(ʣ==null||ʣ.Length==0){return Ϙ;}for(int ó=0;ó<ʣ.Length;ó++){char Ĭ=ʣ[ó];Ϙ^=(
byte)(Ĭ&0xFF);Ϙ*=ϊ;Ϙ^=(byte)((Ĭ>>8)&0xFF);Ϙ*=ϊ;}return Ϙ;}}public static class û{public static bool ü(string ú,string Ʒ){if(
string.IsNullOrEmpty(ú)||string.IsNullOrEmpty(Ʒ))return false;return ú.IndexOf(Ʒ,StringComparison.OrdinalIgnoreCase)>=0;}}
public static class λ{private static readonly StringBuilder ϙ=new StringBuilder(48);public static string μ(string Ƀ){if(string
.IsNullOrEmpty(Ƀ)){return"-";}if(string.Equals(Ƀ,"Iron",ʂ.ʃ)){return"Fe";}if(string.Equals(Ƀ,"Nickel",ʂ.ʃ)){return"Ni";}
if(string.Equals(Ƀ,"Cobalt",ʂ.ʃ)){return"Co";}if(string.Equals(Ƀ,"Silicon",ʂ.ʃ)){return"Si";}if(string.Equals(Ƀ,"Silver",ʂ
.ʃ)){return"Ag";}if(string.Equals(Ƀ,"Gold",ʂ.ʃ)){return"Au";}if(string.Equals(Ƀ,"Magnesium",ʂ.ʃ)){return"Mg";}if(string.
Equals(Ƀ,"Platinum",ʂ.ʃ)){return"Pt";}if(string.Equals(Ƀ,"Uranium",ʂ.ʃ)){return"U";}if(string.Equals(Ƀ,"Stone",ʂ.ʃ)){return
"St";}if(string.Equals(Ƀ,"Ice",ʂ.ʃ)){return"Ic";}if(Ƀ.Length<=2){return Ƀ.ToUpperInvariant();}return Ƀ.Substring(0,2).
ToUpperInvariant();}}public static class ƫ{public static float Ƭ(float Ő,float Ϛ,float ϛ){if(Ϛ>ϛ){float Ϝ=Ϛ;Ϛ=ϛ;ϛ=Ϝ;}if(Ő<Ϛ)return Ϛ;if(
Ő>ϛ)return ϛ;return Ő;}}public static class ʂ{public const StringComparison ʃ=StringComparison.OrdinalIgnoreCase;