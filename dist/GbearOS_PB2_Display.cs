// 
A B;C D;E F;int G=0;public
 Program(){Runtime.UpdateFrequency=UpdateFrequency.Update10;B=new A();D=new C();F=new E();B.H(
GridTerminalSystem,Me,D,F);F.H(this);}public void
 Save
(){}public void
 Main
(string I,UpdateType J){try{if((J&UpdateType.IGC)!=0){K();}if((J&UpdateType.Update10)!=0){L();}}catch(Exception M){string
N="PB2 ERROR:\n"+M.ToString();Echo(N);}}void L(){G++;bool O=(G%10==0);if(!F.P&&O){Echo("NETWORK OFFLINE");}K();double Q;
if(F.R==0){Q=0;}else{Q=(DateTime.UtcNow.Ticks-F.R)/(double)TimeSpan.TicksPerSecond;}string T=F.S??"";string U="=== "+T+
" DISPLAY MANAGER ===\n"+"Last Run: "+Runtime.LastRunTimeMs.ToString("F4")+" ms\n"+"Instructions: "+Runtime.CurrentInstructionCount.ToString()+
" / "+Runtime.MaxInstructionCount.ToString();F.V=U;if(Q>5.0||F.R==0){B.W(Q);if(O){Echo(U);Echo(
"STATUS: NO SIGNAL FROM ORCHESTRATOR");}}else{B.X(F.Y,F.Z,F.a,F.b,F.c,F.d,O);if(O){Echo(U);}}}void K(){F.e();}
}
public interface º{float v(A f,VRageMath.Vector2 g,VRageMath.RectangleF h,string i,j k,l m,n o,p q,r s,t u);void µ(A f,
MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,string i,float y,float z,float ª,j k,l m,n o,p q,r s,t u);}public class E{
MyGridProgram À;string Á="";private readonly IMyBroadcastListener[]Â=new IMyBroadcastListener[8];int Ã,Ä;private readonly Dictionary<
string,long>Å=new Dictionary<string,long>();j Æ=new j();n Ç=new n();p È=new p();r É=new r();l Ê=new l();t Ë=new t();private
readonly Dictionary<long,string>Ì=new Dictionary<long,string>();private readonly Dictionary<long,long>Í=new Dictionary<long,long
>();private readonly List<long>Î=new List<long>();public j Y{get{return Æ;}}public n Z{get{return Ç;}}public p a{get{
return È;}}public r b{get{return É;}}public l c{get{return Ê;}}public t d{get{return Ë;}}public long R{get;private set;}public
bool P{get;private set;}public string S{get;private set;}="";public string V{get;set;}public void H(MyGridProgram Ï){À=Ï;Ã=0
;Ð();Ñ(Ò.Ó);Ñ(Ò.Ô);Ñ(Ò.Õ);Ñ(Ò.Ö);Ñ(Ò.Ø);Ñ(Ò.Ù);Ñ(Ò.Ú);}private static string ã(string Û,string Ü){if(string.IsNullOrEmpty
(Û))return Ü;int Ý=Û.IndexOf('-');string Þ=Ý<0?Û:Û.Substring(0,Ý);char[]ß=new char[3];int à=0;for(int á=0;á<Þ.Length&&à<3
;á++){char â=Þ[á];if(char.IsLetterOrDigit(â)){ß[à]=char.ToUpperInvariant(â);à++;}}if(à==0)return Ü;return new string(ß,0,
à);}string è(string ä,string å){string æ=À.Me.EntityId.ToString("X");æ=æ.Substring(Math.Max(0,æ.Length-4));string ç=ã(ä,å
);return ç+"-"+æ;}void Ð(){IMyProgrammableBlock é=À.Me;var ê=new VRage.Game.ModAPI.Ingame.Utilities.MyIni();VRage.Game.
ModAPI.Ingame.Utilities.MyIniParseResult ë;if(!ê.TryParse(é.CustomData??"",out ë)){ê.Clear();}string ì=ê.Get("Network",
"SharedKey").ToString("");bool í=ê.Get("Network","EnableNetwork").ToBoolean(true);string î=ê.Get("Network","PBID").ToString("");if(
string.IsNullOrWhiteSpace(î))î=ê.Get("Network","SenderId").ToString("");if(î!=null)î=î.Trim();this.S=è(î??"","DIS");ê.Set(
"Network","EnableNetwork",í);ê.SetComment("Network","EnableNetwork",
"See docs/configuration.md — set false for offline mode (no envelope parse).");ê.Set("Network","PBID",this.S);ê.SetComment("Network","PBID","Format: ABC-XXXX. You may change the 3-letter prefix. The 4-character suffix is locked to this block's ID and will auto-reset if changed."
);ê.Set("Network","SharedKey",ì);ê.SetComment("Network","SharedKey","Must match PB1 SharedKey.");é.CustomData=ê.ToString(
);P=í;Á=ì==null?"":ì.Trim();}void Ñ(string ï){IMyBroadcastListener ð=À.IGC.RegisterBroadcastListener(ï);ð.
SetMessageCallback("PB1_MSG");Â[Ã]=ð;Ã++;}public void e(){for(int á=0;á<Ã;á++){IMyBroadcastListener ð=Â[á];while(ð.HasPendingMessage){
MyIGCMessage ñ=ð.AcceptMessage();object ò=ñ.Data;string ó=ò as string;if(ó==null)continue;try{ô(ñ,ó);}catch{}}}}public void û(List<
string>õ){õ.Clear();long ö=System.DateTime.UtcNow.Ticks-30L*System.TimeSpan.TicksPerSecond;Î.Clear();foreach(KeyValuePair<long
,string>ù in Ì){long ø;if(!Í.TryGetValue(ù.Key,out ø)||ø<ö)Î.Add(ù.Key);}for(int á=0;á<Î.Count;á++){long ú=Î[á];Ì.Remove(
ú);Í.Remove(ú);}foreach(KeyValuePair<long,string>ù in Ì)õ.Add(ù.Value);if(!string.IsNullOrEmpty(V))õ.Add(V);}void ô(
MyIGCMessage ñ,string ó){if(string.IsNullOrEmpty(Á)){return;}string ü=ñ.Tag;if(ü==Ò.Ù){long ý=ñ.Source;Í[ý]=System.DateTime.UtcNow.
Ticks;Ì[ý]=ó??string.Empty;return;}string þ;string ÿ;if(!Ā.ā(ó,Á,Å,out ÿ,out þ)){Ä++;return;}if(ü==Ò.Ó){R=System.DateTime.
UtcNow.Ticks;j Ą=Ă.ă<j>(þ);if(Ą!=null)Æ=Ą;return;}if(ü==Ò.Ô){R=System.DateTime.UtcNow.Ticks;n Ą=Ă.ă<n>(þ);if(Ą!=null)Ç=Ą;
return;}if(ü==Ò.Õ){R=System.DateTime.UtcNow.Ticks;p Ą=Ă.ă<p>(þ);if(Ą!=null)È=Ą;return;}if(ü==Ò.Ö){R=System.DateTime.UtcNow.
Ticks;r Ą=Ă.ă<r>(þ);if(Ą!=null)É=Ą;return;}if(ü==Ò.Ø){R=System.DateTime.UtcNow.Ticks;l Ą=Ă.ă<l>(þ);if(Ą!=null)Ê=Ą;return;}if(
ü==Ò.Ú){R=System.DateTime.UtcNow.Ticks;t Ą=Ă.ă<t>(þ);if(Ą!=null)Ë=Ą;return;}}}public class A{sealed class Ć:º{private
readonly A ą;public Ć(A f){ą=f;}public float v(A f,VRageMath.Vector2 g,VRageMath.RectangleF h,string i,j k,l m,n o,p q,r s,t u){
return ć(g,h,i,k,m);}public void µ(A f,MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,string i,float y,float z
,float ª,j k,l m,n o,p q,r s,t u){Ĉ(w,g,x,i,y,z,ª,k,m);}void Đ(string ĉ,l Ċ){ċ(ĉ,Ċ,ą.Č,ą.č,ą.Ď,ą.ď);}private const float
đ=0.55f;private static bool Ē(VRageMath.Vector2 g,VRageMath.RectangleF h){if(g.X<24f)return false;return h.Width<g.X*
0.72f;}float ć(VRageMath.Vector2 g,VRageMath.RectangleF h,string ĉ,j ē,l Ċ){if(ē==null||Ċ==null)return g.Y*0.12f;float Ĕ=g.Y*
0.03515625f;bool ĕ=Ē(g,h);float ė=ĕ?Ė(đ,h.Width):đ;float ę=ĕ?Ę(g.Y,ė,đ):Ĕ;int ě=Ě(h.Width,ė);if(ě<8)ě=8;int ĝ=Ĝ(ě);float Ğ=g.Y*
0.035f+g.Y*0.11f+g.Y*0.02f;float ğ=g.Y*0.02f;bool Ġ=!string.IsNullOrEmpty(ĉ);float ġ=Ġ?0f:(Ğ+g.Y*0.015f);Đ(ĉ,Ċ);int Ģ=ą.Ď.
Count;int ģ=ą.ď.Count;if(ĕ){int Ĥ=0;for(int ĥ=0;ĥ<ģ;ĥ++){string Ħ=ą.ď[ĥ];float ħ=0f;float Ĩ=0f;ą.Č.TryGetValue(Ħ,out ħ);ą.č.
TryGetValue(Ħ,out Ĩ);string ĩ;if(string.Equals(Ħ,"Ice",Ī.ī)){float Ĭ=ħ+Ĩ;ĩ=ĭ.Į(Ĭ)+" "+Ħ;}else{string į=ĭ.Į(ħ);string İ=ĭ.Į(Ĩ);ĩ=į+
"/"+İ+" "+Ħ;}Ĥ+=ı(ĩ,ě,ĝ);}int Ĳ=0;for(int ĳ=0;ĳ<Ģ;ĳ++){int Ĵ=ą.Ď[ĳ];string Ķ=Ċ.ĵ[Ĵ]??"";string ĸ=ĭ.Į(Ċ.ķ[Ĵ]);string Ĺ=ĸ.
PadLeft(6)+" "+Ķ;Ĳ+=ı(Ĺ,ě,ĝ);}bool ĺ=ģ>0||Ģ>0;if(!ĺ)return ġ+ę+ğ;int Ļ;if(Ġ){if(ģ>0&&Ģ>0)Ļ=2;else Ļ=1;}else Ļ=2;return ġ+Ļ*ę+(Ĥ
+Ĳ)*ę+ğ;}float ļ=h.Width;float Ľ=h.X;float ľ=Ľ+ļ*0.01953125f;float Ŀ=Ľ+ļ*0.52f;float ŀ=Math.Max(24f,Ŀ-ľ-2f);float Ł=Math.
Max(24f,(Ľ+ļ)-Ŀ-2f);float ł=Math.Max(40f,ļ-ļ*0.04f);int Ń=Ġ&&Ģ==0?Ě(ł,đ):Ě(ŀ,đ);int ń=Ġ&&ģ==0?Ě(ł,đ):Ě(Ł,đ);if(Ń<8)Ń=8;if(ń
<8)ń=8;int Ņ=Ĝ(Ń);int ņ=Ĝ(ń);int Ň=Math.Max(ģ,Ģ);if(Ň==0)return ġ+Ĕ+ğ;int ň=Ġ?((ģ>0||Ģ>0)?1:0):1;int ŉ=0;for(int ë=0;ë<Ň;
ë++){int Ŋ=0;int ŋ=0;if(ë<ģ){string Ħ=ą.ď[ë];float ħ=0f;float Ĩ=0f;ą.Č.TryGetValue(Ħ,out ħ);ą.č.TryGetValue(Ħ,out Ĩ);
string ĩ;if(string.Equals(Ħ,"Ice",Ī.ī)){float Ĭ=ħ+Ĩ;ĩ=ĭ.Į(Ĭ)+" "+Ħ;}else{string į=ĭ.Į(ħ);string İ=ĭ.Į(Ĩ);ĩ=į+"/"+İ+" "+Ħ;}Ŋ=ı(
ĩ,Ń,Ņ);}if(ë<Ģ){int Ĵ=ą.Ď[ë];string Ķ=Ċ.ĵ[Ĵ]??"";string ĸ=ĭ.Į(Ċ.ķ[Ĵ]);string Ĺ=ĸ.PadLeft(6)+" "+Ķ;ŋ=ı(Ĺ,ń,ņ);}int Ō=Math.
Max(1,Math.Max(Ŋ,ŋ));ŉ+=Ō;}return ġ+ň*Ĕ+ŉ*Ĕ+ğ;}void Ĉ(MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,string
ĉ,float y,float ō,float Ŏ,j ē,l Ċ){if(ē==null||Ċ==null)return;if(Ċ.ĵ==null||Ċ.ķ==null||Ċ.ŏ==null)return;float ļ=x.Width;
float Ľ=x.X;float Ő=ć(g,x,ĉ,ē,Ċ);if(y+Ő<=ō||y>=Ŏ)return;bool ĕ=Ē(g,x);float Ĕ=g.Y*0.03515625f;float ę=Ĕ;float ő=đ;if(ĕ){ő=Ė(đ
,ļ);ę=Ę(g.Y,ő,đ);}bool Ġ=!string.IsNullOrEmpty(ĉ);Đ(ĉ,Ċ);int ģ=ą.ď.Count;int Œ=ą.Ď.Count;float œ=Ŏ+(g.Y*0.01f);if(ĕ){
float Ŕ=Math.Max(2f,ļ*0.02f);float ŕ=Ľ+Ŕ;int Ŗ=Ě(ļ,ő);if(Ŗ<8)Ŗ=8;int ŗ=Ĝ(Ŗ);float Ř=y;if(!Ġ){float ŝ=ē.ř>0.0001f?Ś.ś(ē.Ŝ/ē.ř,
0f,1f):0f;string Š=ĭ.Ş(ē.ş);string Ō=ĭ.Į(ē.Ŝ);string š=ĭ.Į(ē.ř);var Ţ=new[]{"Cargo"};var ţ=new[]{ŝ};var Ť=new[]{Ō+" / "+š+
" L "+Š};var ť=new VRageMath.Color(0,0,255,200);float Ŧ=Ľ+ļ*0.5f;float Ũ=ą.ŧ(y,new VRageMath.Vector2(ļ,g.Y),new VRageMath.
Vector2(Ŧ,0f),Ţ,ţ,Ť,ť,true);Ř=y+Ũ+g.Y*0.015f;ą.ũ("ORES/INGOTS",ŕ,Ř,ő,A.Ū,A.ū,TextAlignment.LEFT);Ř+=ę;for(int ĥ=0;ĥ<ģ;ĥ++){
string Ħ=ą.ď[ĥ];float ħ=0f;float Ĩ=0f;ą.Č.TryGetValue(Ħ,out ħ);ą.č.TryGetValue(Ħ,out Ĩ);string ĩ;if(string.Equals(Ħ,"Ice",Ī.ī)
){float Ĭ=ħ+Ĩ;ĩ=ĭ.Į(Ĭ)+" "+Ħ;}else{string į=ĭ.Į(ħ);string İ=ĭ.Į(Ĩ);ĩ=į+"/"+İ+" "+Ħ;}if(Ř+ę>ō&&Ř<œ){int ů=ą.Ŭ(ĩ,ŕ,Ř,ę,ő,A.
ŭ,A.Ů,TextAlignment.LEFT,Ŗ,ŗ,true);Ř+=ů*ę;}else{int ů=ı(ĩ,Ŗ,ŗ);Ř+=ů*ę;}}ą.ũ("COMPONENTS",ŕ,Ř,ő,A.Ū,A.ū,TextAlignment.LEFT
);Ř+=ę;for(int ĳ=0;ĳ<Œ;ĳ++){int Ĵ=ą.Ď[ĳ];string Ķ=Ċ.ĵ[Ĵ]??"";string ĸ=ĭ.Į(Ċ.ķ[Ĵ]);string Ĺ=ĸ.PadLeft(6)+" "+Ķ;if(Ř+ę>ō&&Ř
<œ){int Ű=ą.Ŭ(Ĺ,ŕ,Ř,ę,ő,A.ŭ,A.Ů,TextAlignment.LEFT,Ŗ,ŗ,true);Ř+=Ű*ę;}else{int Ű=ı(Ĺ,Ŗ,ŗ);Ř+=Ű*ę;}}}else{float ű=y;if(ģ>0
&&Œ>0){ą.ũ("ORES/INGOTS",ŕ,ű,ő,A.Ū,A.ū,TextAlignment.LEFT);ą.ũ("COMPONENTS",ŕ,ű+ę,ő,A.Ū,A.ū,TextAlignment.LEFT);Ř=ű+ę*2f;}
else if(ģ>0){ą.ũ("ORES/INGOTS",ŕ,ű,ő,A.Ū,A.ū,TextAlignment.LEFT);Ř=ű+ę;}else if(Œ>0){ą.ũ("COMPONENTS",ŕ,ű,ő,A.Ū,A.ū,
TextAlignment.LEFT);Ř=ű+ę;}else Ř=y;for(int ĥ=0;ĥ<ģ;ĥ++){string Ħ=ą.ď[ĥ];float ħ=0f;float Ĩ=0f;ą.Č.TryGetValue(Ħ,out ħ);ą.č.
TryGetValue(Ħ,out Ĩ);string ĩ;if(string.Equals(Ħ,"Ice",Ī.ī)){float Ĭ=ħ+Ĩ;ĩ=ĭ.Į(Ĭ)+" "+Ħ;}else{string į=ĭ.Į(ħ);string İ=ĭ.Į(Ĩ);ĩ=į+
"/"+İ+" "+Ħ;}if(Ř+ę>ō&&Ř<œ){int Ō=ą.Ŭ(ĩ,ŕ,Ř,ę,ő,A.ŭ,A.Ů,TextAlignment.LEFT,Ŗ,ŗ,true);Ř+=Ō*ę;}else{int Ō=ı(ĩ,Ŗ,ŗ);Ř+=Ō*ę;}}
for(int ĳ=0;ĳ<Œ;ĳ++){int Ĵ=ą.Ď[ĳ];string Ķ=Ċ.ĵ[Ĵ]??"";string ĸ=ĭ.Į(Ċ.ķ[Ĵ]);string Ĺ=ĸ.PadLeft(6)+" "+Ķ;if(Ř+ę>ō&&Ř<œ){int Ō
=ą.Ŭ(Ĺ,ŕ,Ř,ę,ő,A.ŭ,A.Ů,TextAlignment.LEFT,Ŗ,ŗ,true);Ř+=Ō*ę;}else{int Ō=ı(Ĺ,Ŗ,ŗ);Ř+=Ō*ę;}}}return;}float Ų;if(!Ġ){float ŝ=
ē.ř>0.0001f?Ś.ś(ē.Ŝ/ē.ř,0f,1f):0f;string Š=ĭ.Ş(ē.ş);string Ō=ĭ.Į(ē.Ŝ);string š=ĭ.Į(ē.ř);var Ţ=new[]{"Cargo"};var ţ=new[]{
ŝ};var Ť=new[]{Ō+" / "+š+" L "+Š};var ť=new VRageMath.Color(0,0,255,200);float Ŧ=Ľ+ļ*0.5f;float Ũ=ą.ŧ(y,new VRageMath.
Vector2(ļ,g.Y),new VRageMath.Vector2(Ŧ,0f),Ţ,ţ,Ť,ť,true);float ų=y+Ũ+g.Y*0.015f;float Ŵ=Ľ+ļ*0.01953125f;float ŵ=Ľ+ļ*0.52f;ą.ũ(
"ORES/INGOTS",Ŵ,ų,đ,A.Ū,A.ū,TextAlignment.LEFT);ą.ũ("COMPONENTS",ŵ,ų,đ,A.Ū,A.ū,TextAlignment.LEFT);Ų=ų+Ĕ;}else{float ű=y;if(ģ>0&&Œ>0)
{ą.ũ("ORES/INGOTS",Ľ+ļ*0.01953125f,ű,đ,A.Ū,A.ū,TextAlignment.LEFT);ą.ũ("COMPONENTS",Ľ+ļ*0.52f,ű,đ,A.Ū,A.ū,TextAlignment.
LEFT);Ų=ű+Ĕ;}else if(ģ>0){ą.ũ("ORES/INGOTS",Ľ+ļ*0.01953125f,ű,đ,A.Ū,A.ū,TextAlignment.LEFT);Ų=ű+Ĕ;}else if(Œ>0){ą.ũ(
"COMPONENTS",Ľ+ļ*0.01953125f,ű,đ,A.Ū,A.ū,TextAlignment.LEFT);Ų=ű+Ĕ;}else Ų=y;}float ľ=Ľ+ļ*0.01953125f;float Ŀ=Ľ+ļ*0.52f;float ŀ=Math
.Max(24f,Ŀ-ľ-2f);float Ł=Math.Max(24f,(Ľ+ļ)-Ŀ-2f);float ł=Math.Max(40f,ļ-ļ*0.04f);int Ń=Ġ&&Œ==0?Ě(ł,đ):Ě(ŀ,đ);int ń=Ġ&&ģ
==0?Ě(ł,đ):Ě(Ł,đ);if(Ń<8)Ń=8;if(ń<8)ń=8;int Ņ=Ĝ(Ń);int ņ=Ĝ(ń);float Ŷ=Ų;int Ň=Math.Max(ģ,Œ);float ŷ=Ŷ;for(int ë=0;ë<Ň;ë++)
{string Ÿ=null;string Ź=null;int Ŋ=0;int ŋ=0;if(ë<ģ){string Ħ=ą.ď[ë];float ħ=0f;float Ĩ=0f;ą.Č.TryGetValue(Ħ,out ħ);ą.č.
TryGetValue(Ħ,out Ĩ);if(string.Equals(Ħ,"Ice",Ī.ī)){float Ĭ=ħ+Ĩ;Ÿ=ĭ.Į(Ĭ)+" "+Ħ;}else{string į=ĭ.Į(ħ);string İ=ĭ.Į(Ĩ);Ÿ=į+"/"+İ+" "+
Ħ;}Ŋ=ı(Ÿ,Ń,Ņ);}if(ë<Œ){int Ĵ=ą.Ď[ë];string Ķ=Ċ.ĵ[Ĵ]??"";string ĸ=ĭ.Į(Ċ.ķ[Ĵ]);Ź=ĸ.PadLeft(6)+" "+Ķ;ŋ=ı(Ź,ń,ņ);}int ź=Math.
Max(1,Math.Max(Ŋ,ŋ));if(ŷ+Ĕ>ō&&ŷ<œ){if(Ÿ!=null){float Ż=Ġ&&Œ==0?Ľ+ļ*0.01953125f:ľ;ą.Ŭ(Ÿ,Ż,ŷ,Ĕ,đ,A.ŭ,A.Ů,TextAlignment.LEFT,
Ń,Ņ,true);}if(Ź!=null){float ż=Ġ&&ģ==0?Ľ+ļ*0.01953125f:Ŀ;ą.Ŭ(Ź,ż,ŷ,Ĕ,đ,A.ŭ,A.Ů,TextAlignment.LEFT,ń,ņ,true);}}ŷ+=ź*Ĕ;}}}
private const string Ž="[GbearOS]",ž="[Manual]",ū="White",Ů="Monospace",ſ="SquareSimple";private static readonly VRageMath.
Color ŭ=VRageMath.Color.White,ƀ=new VRageMath.Color(255,0,0,255),Ɓ=new VRageMath.Color(0,255,0,255),Ū=new VRageMath.Color(128
,128,128,255),Ƃ=new VRageMath.Color(0,0,0,255),ƃ=new VRageMath.Color(38,42,48,255);enum ƍ{Ƅ,ƅ,Ɔ,Ƈ,ƈ,Ɖ,Ɗ,Ƌ,ƌ,}struct Ƒ{
public ƍ Ǝ;public string Ə,Ɛ;}struct ƚ{public IMyTextPanel ƒ;public List<Ƒ>Ɠ;public float Ɣ,ƕ,Ɩ,Ɨ;public int Ƙ;public bool ƙ;}
IMyGridTerminalSystem ƛ;IMyProgrammableBlock À;C D;private readonly List<IMyTextPanel>Ɯ=new List<IMyTextPanel>(64);private readonly List<ƚ>Ɲ=
new List<ƚ>(64),ƞ=new List<ƚ>(64);private readonly List<MySprite>Ɵ=new List<MySprite>(320);private readonly Ơ ơ=new Ơ(),Ƣ=
new Ơ();private readonly List<string>ƣ=new List<string>(8),ď=new List<string>(128),Ƥ=new List<string>(16),ƥ=new List<string
>(16);private readonly Dictionary<string,float>Č=new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase),č=new
Dictionary<string,float>(StringComparer.OrdinalIgnoreCase);private readonly List<int>Ď=new List<int>(128);E F;bool Ʀ,Ƨ,ƨ,Ʃ,ƪ,ƫ;int
Ƭ=0;float ƭ=-1f;float Ʈ=9999f;j Ư;n ư;p Ʊ;r Ʋ;l Ƴ;t ƴ;bool Ƶ;Dictionary<string,º>ƶ;private static void Ƹ(IMyTextPanel Ʒ){
if(Ʒ==null)return;Ʒ.ContentType=ContentType.SCRIPT;Ʒ.Script="";Ʒ.ScriptBackgroundColor=VRageMath.Color.Black;}private
static void ƻ(MySpriteDrawFrame w,VRageMath.Vector2 ƹ,VRageMath.Vector2 ƺ){w.Add(new MySprite{Type=SpriteType.TEXTURE,Data=ſ,
Position=ƺ,Size=ƹ,Color=Ƃ,Alignment=TextAlignment.CENTER,RotationOrScale=0f,});}void ũ(string Ƽ,float ƽ,float ƾ,float ƿ,
VRageMath.Color ĳ,string ǀ,TextAlignment ǁ){if(ƭ>=0f&&(ƾ<ƭ||ƾ>Ʈ))return;Ɵ.Add(new MySprite{Type=SpriteType.TEXT,Data=Ƽ,Position=
new VRageMath.Vector2(ƽ,ƾ),Color=ĳ,FontId=ǀ,Alignment=ǁ,RotationOrScale=ƿ,});}void ǅ(string ǂ,float ƽ,float ƾ,float ǃ,float
Ǆ,VRageMath.Color ĳ){if(ƭ>=0f&&(ƾ-(Ǆ*0.5f)<ƭ||ƾ+(Ǆ*0.5f)>Ʈ))return;Ɵ.Add(new MySprite{Type=SpriteType.TEXTURE,Data=ǂ,
Position=new VRageMath.Vector2(ƽ,ƾ),Size=new VRageMath.Vector2(ǃ,Ǆ),Color=ĳ,Alignment=TextAlignment.CENTER,RotationOrScale=0f,})
;}void Ǎ(float ǆ,float ƾ,float Ǉ,float ǈ,float ǉ,float ŝ,VRageMath.Color Ǌ){ǅ(ſ,ǆ,ƾ,Ǉ,ǈ,ƃ);float ë=Ś.ś(ŝ,0f,1f);if(ë<=
1e-5f)return;float ǋ=Ś.ś(Math.Max(ǉ,ë*Ǉ),ǉ,Ǉ);float ǌ=ǆ-Ǉ*0.5f+ǋ*0.5f;ǅ(ſ,ǌ,ƾ,ǋ,ǈ,Ǌ);}float ŧ(float y,VRageMath.Vector2 ƹ,
VRageMath.Vector2 ƺ,string[]ǎ,float[]Ǐ,string[]ǐ,VRageMath.Color Ǒ,bool ǒ){float Ǔ=ƹ.Y*0.11f;float ǈ=ƹ.Y*0.045f;float ǔ=ƹ.X*0.02f
;float Ǉ=ƹ.X-2f*ǔ;float Ǖ=ǈ*0.35f;int ǖ=ǎ.Length;float Ǘ=y+ƹ.Y*0.035f;for(int á=0;á<ǖ;á++){float ƾ=Ǘ+á*Ǔ;if(ǒ)Ǎ(ƺ.X,ƾ,Ǉ,ǈ
,Ǖ,Ǐ[á],Ǒ);if(ǒ)ũ(ǎ[á]+" "+ǐ[á],ƺ.X,ƾ+ǈ*0.55f,0.55f,new VRageMath.Color(230,230,230,255),ū,TextAlignment.CENTER);}return
ƹ.Y*0.035f+ǖ*Ǔ+ƹ.Y*0.02f;}public void H(IMyGridTerminalSystem ǘ,IMyProgrammableBlock Ï,C Ǚ,E ǚ){ƛ=ǘ;À=Ï;D=Ǚ;F=ǚ;Ƭ=0;ƶ=new
Dictionary<string,º>(StringComparer.OrdinalIgnoreCase);ƶ["INV"]=new Ć(this);ƶ["PWR"]=new Ǜ(this);ƶ["ICE"]=new ǜ(this);ƶ["REF"]=new
ǝ(this);ƶ["WARN"]=new Ǟ(this);ƶ["STATUS"]=new ǟ(this);}º Ǣ(string Ǡ){if(ƶ==null||Ǡ==null)return null;º ǡ;return ƶ.
TryGetValue(Ǡ,out ǡ)?ǡ:null;}private static ƍ ǥ(string ǣ){if(string.IsNullOrEmpty(ǣ))return ƍ.Ƅ;if(string.Equals(ǣ,"HEAD",Ī.ī))
return ƍ.ƅ;if(string.Equals(ǣ,"INV",Ī.ī))return ƍ.Ɔ;if(string.Equals(ǣ,"REF",Ī.ī))return ƍ.Ƈ;if(string.Equals(ǣ,"PWR",Ī.ī))
return ƍ.ƈ;if(string.Equals(ǣ,"ICE",Ī.ī))return ƍ.Ɖ;if(string.Equals(ǣ,"WARN",Ī.ī))return ƍ.Ɗ;if(string.Equals(ǣ,"STATUS",Ī.ī)
)return ƍ.Ƌ;if(string.Equals(ǣ,Ơ.Ǥ,Ī.ī))return ƍ.ƌ;return ƍ.Ƅ;}private static string ǧ(ref Ƒ Ǧ){if(Ǧ.Ǝ==ƍ.Ƅ)return Ǧ.Ɛ;
switch(Ǧ.Ǝ){case ƍ.Ɔ:return"INV";case ƍ.Ƈ:return"REF";case ƍ.ƈ:return"PWR";case ƍ.Ɖ:return"ICE";case ƍ.Ɗ:return"WARN";case ƍ.Ƌ
:return"STATUS";default:return null;}}private static string ǩ(ƍ Ƽ,string Ǩ){switch(Ƽ){case ƍ.Ɔ:return"INVENTORY";case ƍ.Ƈ
:return"REFINERY STATUS";case ƍ.Ɖ:return"ICE STATUS";case ƍ.ƈ:return"POWER GRID STATUS";case ƍ.Ɗ:return"WARNING STATUS";
case ƍ.Ƌ:return"SYSTEM STATUS";case ƍ.Ƅ:return Ǩ!=null?Ǩ:"";default:return"";}}public void W(double Ǫ){if(ƛ==null||À==null)
return;ǫ();string Ǭ="Offline for: "+Ǫ.ToString("F0")+"s";int ǭ=Ɲ.Count;for(int á=0;á<ǭ;á++){IMyTextPanel Ǯ=Ɲ[á].ƒ;if(Ǯ==null)
continue;Ƹ(Ǯ);VRageMath.Vector2 ƹ;VRageMath.Vector2 ƺ;ǯ(Ǯ,out ƹ,out ƺ);using(var w=Ǯ.DrawFrame()){ƻ(w,ƹ,ƺ);Ɵ.Clear();ũ(
"NO SIGNAL",ƺ.X,ƹ.Y*0.10f,1.35f,ƀ,ū,TextAlignment.CENTER);ũ("WAITING FOR TELEMETRY...",ƺ.X,ƹ.Y*0.20f,0.72f,ŭ,ū,TextAlignment.CENTER
);ũ(Ǭ,ƺ.X,ƹ.Y*0.28f,0.62f,Ū,ū,TextAlignment.CENTER);ǰ(w);}}}public void X(j k,n o,p q,r s,l m,t u,bool O){if(ƛ==null||À==
null)return;ǫ();if(O){Ƶ=Ǳ(Ư,k);Ƨ=Ǳ(ư,o);ƨ=Ǳ(Ʊ,q);Ʃ=Ǳ(Ʋ,s);ƪ=Ǳ(Ƴ,m);ƫ=Ǳ(ƴ,u);Ʀ=ǲ();Ư=k;ư=o;Ʊ=q;Ʋ=s;Ƴ=m;ƴ=u;}ǳ(k,o,q,s,m,u);Ǵ(
k,o,q,s,m,u);}private static bool Ǳ<ǵ>(ǵ ǁ,ǵ Ƕ){if(ǁ==null&&Ƕ==null)return false;if(ǁ==null||Ƕ==null)return true;return!ǁ
.Equals(Ƕ);}bool Ǻ(List<Ƒ>Ƿ){if(Ƿ==null||Ƿ.Count==0)return false;bool Ǹ=Ƶ||ƪ||Ƨ||ƨ||Ʃ||ƫ||Ʀ;bool ǹ=Ƶ||ƪ;int ǖ=Ƿ.Count;for
(int á=0;á<ǖ;á++){switch(Ƿ[á].Ǝ){case ƍ.Ɔ:if(ǹ)return true;break;case ƍ.Ƈ:if(Ƨ)return true;break;case ƍ.Ɖ:if(ƨ)return
true;break;case ƍ.ƈ:if(Ʃ)return true;break;case ƍ.Ɗ:if(ƫ)return true;break;case ƍ.Ƌ:if(Ʀ)return true;break;case ƍ.Ƅ:if(Ǹ)
return true;break;}}return false;}void ǰ(MySpriteDrawFrame w){int ǖ=Ɵ.Count;for(int ǻ=0;ǻ<ǖ;ǻ++)w.Add(Ɵ[ǻ]);Ɵ.Clear();}void Ǵ(
j k,n o,p q,r s,l m,t u){int ǭ=Ɲ.Count;for(int á=0;á<ǭ;á++){var M=Ɲ[á];if(M.Ɠ==null||M.Ɠ.Count==0)continue;if(!Ǻ(M.Ɠ)&&!M
.ƙ)continue;Ǽ(ref M,k,o,q,s,m,u);M.ƙ=false;Ɲ[á]=M;}}void ǳ(j k,n o,p q,r s,l m,t u){int ǭ=Ɲ.Count;for(int á=0;á<ǭ;á++){
var M=Ɲ[á];if(M.Ɠ==null)continue;VRageMath.Vector2 ǽ,Ǿ;ǯ(M.ƒ,out ǽ,out Ǿ);float ǿ=ǽ.Y*0.95703125f;float Ȁ,ȁ;Ȃ(ơ,M.Ɠ,ǽ,k,o,q
,s,m,u,out Ȁ,out ȁ);M.Ɩ=Ȁ;M.Ɨ=ȁ;float ȃ=ǿ-Ȁ;if(ȁ>ȃ){float Ȅ=ȁ-ȃ;float ȅ=ȃ*0.90f;if(M.ƕ>M.Ɣ){float Ȇ=ȅ/12f;M.Ɣ+=Ȇ;if(M.Ɣ>=
M.ƕ)M.Ɣ=M.ƕ;M.ƙ=true;}else if(M.ƕ<M.Ɣ){float ȇ=M.Ɣ-M.ƕ;float Ȉ=ȇ*0.15f;if(Ȉ<20f)Ȉ=20f;M.Ɣ-=Ȉ;if(M.Ɣ<=M.ƕ)M.Ɣ=M.ƕ;M.ƙ=true
;}else{M.Ƙ++;if(M.Ƙ>=30){M.Ƙ=0;if(M.Ɣ>=Ȅ-5f){M.ƕ=0f;}else{M.ƕ=M.Ɣ+ȅ;if(M.ƕ>Ȅ)M.ƕ=Ȅ;}M.ƙ=true;}}}else{M.Ɣ=0f;M.ƕ=0f;M.Ƙ=0;
}Ɲ[á]=M;}}void ǫ(){if(Ƭ>0){Ƭ--;return;}Ƭ=100;Ɯ.Clear();ƛ.GetBlocksOfType(Ɯ,ȉ);ƞ.Clear();for(int Ȋ=0;Ȋ<Ɲ.Count;Ȋ++)ƞ.Add(Ɲ
[Ȋ]);Ɲ.Clear();int ǖ=Ɯ.Count;for(int á=0;á<ǖ;á++){var Ʒ=Ɯ[á];if(Ʒ==null)continue;string Ķ=Ʒ.CustomName;if(ȋ.Ȍ(Ķ,ž))
continue;ƚ M;M.ƒ=Ʒ;M.Ɣ=0f;M.ƕ=0f;M.Ƙ=0;M.ƙ=false;M.Ɩ=0f;M.Ɨ=0f;for(int ȍ=0;ȍ<ƞ.Count;ȍ++){if(ƞ[ȍ].ƒ==Ʒ){M.Ɣ=ƞ[ȍ].Ɣ;M.ƕ=ƞ[ȍ].ƕ;M.
Ƙ=ƞ[ȍ].Ƙ;break;}}if(!ȋ.Ȍ(Ķ,Ž))continue;var Ƿ=new List<Ƒ>(8);Ȏ(Ʒ.CustomData,Ƿ);if(Ƿ.Count==0)continue;M.Ɠ=Ƿ;Ɲ.Add(M);}}
void Ȏ(string ȏ,List<Ƒ>õ){õ.Clear();bool Ȑ=string.IsNullOrWhiteSpace(ȏ);if(Ȑ){õ.Add(new Ƒ{Ǝ=ƍ.Ɔ,Ə="",Ɛ=null});return;}int ȑ=
0;int à=ȏ.Length;while(ȑ<à){int Ȓ=ȏ.IndexOf('\n',ȑ);string ȓ=Ȓ<0?ȏ.Substring(ȑ):ȏ.Substring(ȑ,Ȓ-ȑ);ȑ=Ȓ<0?à:Ȓ+1;int Ţ=ȓ.
IndexOf('[');int Ȕ=ȓ.IndexOf(']');if(Ţ<0||Ȕ<=Ţ)continue;string ȕ=ȓ.Substring(Ţ+1,Ȕ-Ţ-1).Trim();if(ȕ.Length==0)continue;Ƒ Ȗ;int
ĳ=ȕ.IndexOf(':');string ȗ;if(ĳ<0){ȗ=ȕ.Trim();Ȗ.Ə="";}else{ȗ=ȕ.Substring(0,ĳ).Trim();Ȗ.Ə=ȕ.Substring(ĳ+1).Trim();}if(ȗ.
Length==0)continue;Ȗ.Ǝ=ǥ(ȗ);if(Ȗ.Ǝ==ƍ.Ƅ)Ȗ.Ɛ=ȗ;else Ȗ.Ɛ=null;õ.Add(Ȗ);}}bool ȉ(IMyTextPanel Ʒ){if(Ʒ==null)return false;if(!Ʒ.
IsSameConstructAs(À))return false;return true;}private static void ǯ(IMyTextPanel Ǯ,out VRageMath.Vector2 ƹ,out VRageMath.Vector2 ƺ){var
Ș=Ǯ as IMyTextSurface;var ș=Ș!=null?Ș.TextureSize:default(VRageMath.Vector2);var Ț=Ș!=null?Ș.SurfaceSize:default(
VRageMath.Vector2);ƹ=(ș.X>=8f&&ș.Y>=8f)?ș:((Ț.X>=8f&&Ț.Y>=8f)?Ț:new VRageMath.Vector2(512f,512f));ƺ=ƹ*0.5f;}float ț(VRageMath.
Vector2 ƹ){return ƹ.Y*0.045f;}float Ȟ(float Ȝ,VRageMath.Vector2 ƹ,float ǆ,string ȝ,bool ǒ){float Ǆ=ț(ƹ);if(ǒ)ũ("--- "+ȝ+" ---",
ǆ,Ȝ,0.55f,Ū,ū,TextAlignment.CENTER);return Ǆ;}void Ȃ(Ơ ȟ,List<Ƒ>Ƿ,VRageMath.Vector2 ƹ,j k,n o,p q,r s,l m,t u,out float ō
,out float Ƞ){ō=ƹ.Y*0.02f;ȟ.ȡ(ƹ.X,ƹ.Y);int Ȣ=Ƿ.Count;for(int á=0;á<Ȣ;á++){var ĳ=Ƿ[á];switch(ĳ.Ǝ){case ƍ.ƅ:ō+=ƹ.Y*0.07f;
continue;case ƍ.ƌ:ȟ.ȣ(ĳ.Ə);continue;}bool Ȥ=(ĳ.Ǝ==ƍ.Ɔ||ĳ.Ǝ==ƍ.Ƌ)&&!string.IsNullOrEmpty(ĳ.Ə);float ȥ=Ȥ?0f:ț(ƹ);float ȧ=Ȧ(ĳ,ȟ,ƹ,k
,o,q,s,m,u);ȟ.Ȩ(ȥ+ȧ);}ȟ.ȩ();Ƞ=ȟ.Ȫ;}float Ȧ(Ƒ Ǧ,Ơ ȟ,VRageMath.Vector2 ƹ,j k,n o,p q,r s,l m,t u){if(Ǧ.Ǝ==ƍ.ƌ)return 0f;
string ȫ=ǧ(ref Ǧ);º ǡ=Ǣ(ȫ);if(ǡ!=null)return ǡ.v(this,ƹ,ȟ.Ȭ,Ǧ.Ə,k,m,o,q,s,u);return ƹ.Y*0.04f;}void Ǽ(ref ƚ ȭ,j k,n o,p q,r s,
l m,t u){IMyTextPanel Ǯ=ȭ.ƒ;if(Ǯ==null)return;Ƹ(Ǯ);VRageMath.Vector2 ƹ;VRageMath.Vector2 ƺ;ǯ(Ǯ,out ƹ,out ƺ);float Ŏ=ƹ.Y*
0.95703125f;float ō=ȭ.Ɩ;float Ȯ=ȭ.Ɨ;float ȯ=Ŏ-ō;float Ȱ=ƹ.Y*0.02f;float ȱ=ō+Ȱ-ȭ.Ɣ;using(var w=Ǯ.DrawFrame()){ƻ(w,ƹ,ƺ);Ɵ.Clear();
float Ȳ=ƹ.Y*0.025f;int Ȣ=ȭ.Ɠ.Count;for(int á=0;á<Ȣ;á++){var ĳ=ȭ.Ɠ[á];if(ĳ.Ǝ!=ƍ.ƅ)continue;string ȳ=string.IsNullOrEmpty(ĳ.Ə)?
" ":ĳ.Ə;ũ(ȳ,ƺ.X,Ȳ,0.88f,ŭ,ū,TextAlignment.CENTER);Ȳ+=ƹ.Y*0.07f;}if(Ȯ>ȯ){float ȴ=Ȯ-ȯ;float ȵ=ȯ*0.90f;int ȶ=(int)Math.Ceiling
(ȴ/ȵ)+1;int ȷ;if(ȭ.Ɣ>=ȴ-5f)ȷ=ȶ;else ȷ=(int)(ȭ.Ɣ/ȵ)+1;ũ("PAGE "+ȷ+"/"+ȶ,ƹ.X*0.97f,ƹ.Y*0.025f,0.5f,new VRageMath.Color(180,
180,180,255),ū,TextAlignment.RIGHT);}ƭ=ō+Ȱ;Ʈ=Ŏ;Ƣ.ȡ(ƹ.X,ƹ.Y);for(int á=0;á<Ȣ;á++){var ĳ=ȭ.Ɠ[á];switch(ĳ.Ǝ){case ƍ.ƅ:continue
;case ƍ.ƌ:Ƣ.ȣ(ĳ.Ə);continue;}bool Ȥ=(ĳ.Ǝ==ƍ.Ɔ||ĳ.Ǝ==ƍ.Ƌ)&&!string.IsNullOrEmpty(ĳ.Ə);float ȥ=Ȥ?0f:ț(ƹ);float ȧ=Ȧ(ĳ,Ƣ,ƹ,k,
o,q,s,m,u);float ȹ=ȱ+Ƣ.ȸ;float Ⱥ=ȹ+ȥ+ȧ;bool Ȼ=Ⱥ<=ō||ȹ>=Ŏ;if(!Ȼ){if(!Ȥ)Ȟ(ȹ,ƹ,Ƣ.ȼ,ǩ(ĳ.Ǝ,ĳ.Ɛ),true);float Ǘ=ȹ+ȥ;Ƚ(ĳ,Ƣ,w,k,o,
q,s,m,u,ƹ,Ǘ,ō,Ŏ);}Ƣ.Ȩ(ȥ+ȧ);}Ƣ.ȩ();ƭ=-1f;ǰ(w);}}void Ƚ(Ƒ Ǧ,Ơ ȟ,MySpriteDrawFrame w,j k,n o,p q,r s,l m,t u,VRageMath.
Vector2 ƹ,float y,float ō,float Ŏ){string ȫ=ǧ(ref Ǧ);º ǡ=Ǣ(ȫ);if(ǡ!=null){ǡ.µ(this,w,ƹ,ȟ.Ȭ,Ǧ.Ə,y,ō,Ŏ,k,m,o,q,s,u);}}bool ǲ(){if
(F==null)return false;F.û(Ƥ);bool Ⱦ=Ƥ.Count!=ƥ.Count;if(!Ⱦ){for(int á=0;á<Ƥ.Count;á++){string ǁ=Ƥ[á]??"";string Ƕ=á<ƥ.
Count?(ƥ[á]??""):"";if(!string.Equals(ǁ,Ƕ,Ī.ī)){Ⱦ=true;break;}}}if(!Ⱦ)return false;ƥ.Clear();for(int á=0;á<Ƥ.Count;á++)ƥ.Add(
Ƥ[á]??"");return true;}private const float ȿ=0.45f;private const int ɀ=2;private const string Ɂ="  ";internal static int
Ě(float ɂ,float Ƀ){float Ʉ=ɂ*0.80f;if(Ʉ<8f)Ʉ=Math.Max(1f,ɂ*0.5f);float Ʌ=19.5f*Ƀ;if(Ʌ<=0.0001f)return 4;int ǖ=(int)(Ʉ/Ʌ);
return ǖ<1?1:ǖ;}internal static float Ė(float Ɇ,float ɇ){float ǃ=ɇ>2f?ɇ:400f;float Ɉ=520f;float ɉ=Ɇ*Math.Min(1f,ǃ/Ɉ);if(ɉ<ȿ)ɉ=
ȿ;if(ɉ>Ɇ)ɉ=Ɇ;return ɉ;}internal static float Ę(float Ɋ,float ė,float Ɇ){float ŝ=Ɇ>1e-4f?ė/Ɇ:1f;ŝ=Math.Max(0.88f,ŝ);return
Ɋ*(0.028f+0.012f*ŝ);}internal static int Ĝ(int ě){int ǖ=ě-ɀ;return ǖ<4?Math.Max(1,ě-1):ǖ;}internal static int ı(string ó,
int ě,int ĝ){if(string.IsNullOrEmpty(ó))return 0;int á=0;int ɋ=0;bool Ɍ=true;while(á<ó.Length){while(á<ó.Length&&ó[á]==' ')
á++;if(á>=ó.Length)break;int ɍ=Ɍ?ě:ĝ;Ɍ=false;int Ɏ=0;while(á<ó.Length){while(á<ó.Length&&ó[á]==' ')á++;if(á>=ó.Length)
break;int ɏ=á;while(á<ó.Length&&ó[á]!=' ')á++;int ɐ=á-ɏ;if(ɐ<=0)continue;int ɑ=Ɏ==0?ɐ:(1+ɐ);if(Ɏ+ɑ<=ɍ){Ɏ+=ɑ;continue;}if(Ɏ==0
){int Ȇ=ɍ<1?1:ɍ;int ɒ=á;int ɓ=ɏ;while(ɓ<ɒ){int ɔ=Math.Min(Ȇ,ɒ-ɓ);ɓ+=ɔ;ɋ++;Ɍ=false;}}else{á=ɏ;ɋ++;Ɍ=false;}goto ɕ;}ɋ++;Ɍ=
false;ɕ:;}return ɋ;}internal int Ŭ(string ó,float Ż,float ɖ,float ɗ,float ɘ,VRageMath.Color ə,string ɚ,TextAlignment ɛ,int ě,
int ĝ,bool ɜ){if(string.IsNullOrEmpty(ó)){ũ(" ",Ż,ɖ,ɘ,ə,ɚ,ɛ);return 1;}int á=0;int ɋ=0;bool Ɍ=true;float ƾ=ɖ;while(á<ó.
Length){while(á<ó.Length&&ó[á]==' ')á++;if(á>=ó.Length)break;int ɍ=Ɍ?ě:ĝ;int ɝ=á;int ɞ=á;int Ɏ=0;while(á<ó.Length){while(á<ó.
Length&&ó[á]==' ')á++;if(á>=ó.Length)break;int ɏ=á;while(á<ó.Length&&ó[á]!=' ')á++;int ɒ=á;int ɐ=ɒ-ɏ;if(ɐ<=0)continue;int ɑ=Ɏ
==0?ɐ:(1+ɐ);if(Ɏ+ɑ<=ɍ){Ɏ+=ɑ;ɞ=ɒ;continue;}if(Ɏ==0){int ɔ=ɍ<1?1:ɍ;ɞ=ɏ+ɔ;á=ɞ;}else{á=ɏ;}break;}string ɟ=ó.Substring(ɝ,Math.
Max(0,ɞ-ɝ)).TrimEnd();if(!Ɍ&&ɜ&&ɟ.Length>0)ɟ=Ɂ+ɟ;if(ɟ.Length==0)ɟ=" ";ũ(ɟ,Ż,ƾ,ɘ,ə,ɚ,ɛ);ƾ+=ɗ;ɋ++;Ɍ=false;}if(ɋ==0){ũ(" ",Ż,ɖ
,ɘ,ə,ɚ,ɛ);return 1;}return ɋ;}internal float ɢ(VRageMath.Vector2 g,VRageMath.RectangleF h,string i,float Ƀ){if(F==null)
return g.Y*0.06f;F.û(Ƥ);float ė=Ė(Ƀ,h.Width);float ɗ=Ę(g.Y,ė,Ƀ);int ě=Ě(h.Width,ė);int ĝ=Ĝ(ě);int ɋ=0;for(int ɠ=0;ɠ<Ƥ.Count;ɠ
++){string Ƕ=Ƥ[ɠ];if(string.IsNullOrEmpty(Ƕ))continue;if(!string.IsNullOrEmpty(i)&&Ƕ.IndexOf(i,Ī.ī)<0)continue;if(ɋ>0)ɋ++;
int ȑ=0;while(ȑ<=Ƕ.Length){int Ȓ=Ƕ.IndexOf('\n',ȑ);string ɡ=Ȓ<0?Ƕ.Substring(ȑ):Ƕ.Substring(ȑ,Ȓ-ȑ);if(ɡ.Length==0)ɋ++;else ɋ
+=ı(ɡ,ě,ĝ);if(Ȓ<0)break;ȑ=Ȓ+1;}}if(ɋ==0)ɋ=1;return ɋ*ɗ+g.Y*0.02f;}internal void ɣ(VRageMath.Vector2 g,VRageMath.RectangleF
x,string i,float y,float z,float ª,float Ƀ){if(F==null)return;float Ő=ɢ(g,x,i,Ƀ);if(y+Ő<=z||y>=ª)return;F.û(Ƥ);float ė=Ė(
Ƀ,x.Width);float ɗ=Ę(g.Y,ė,Ƀ);int ě=Ě(x.Width,ė);int ĝ=Ĝ(ě);float Ż=x.X+x.Width*0.04f;float ƾ=y;bool ĺ=false;for(int ɠ=0;
ɠ<Ƥ.Count;ɠ++){string Ƕ=Ƥ[ɠ];if(string.IsNullOrEmpty(Ƕ))continue;if(!string.IsNullOrEmpty(i)&&Ƕ.IndexOf(i,Ī.ī)<0)continue
;ĺ=true;if(ƾ>y+0.5f)ƾ+=ɗ;int ȑ=0;while(ȑ<=Ƕ.Length){int Ȓ=Ƕ.IndexOf('\n',ȑ);string ɡ=Ȓ<0?Ƕ.Substring(ȑ):Ƕ.Substring(ȑ,Ȓ-ȑ
);if(ɡ.Length==0){if(ƾ+ɗ>z&&ƾ<ª)ũ(" ",Ż,ƾ,ė,Ū,Ů,TextAlignment.LEFT);ƾ+=ɗ;}else{if(ƾ+ɗ>z&&ƾ<ª){int Ō=Ŭ(ɡ,Ż,ƾ,ɗ,ė,ŭ,Ů,
TextAlignment.LEFT,ě,ĝ,true);ƾ+=Ō*ɗ;}else{int Ō=ı(ɡ,ě,ĝ);ƾ+=Ō*ɗ;}}if(Ȓ<0)break;ȑ=Ȓ+1;}}if(!ĺ&&ƾ+ɗ>z&&ƾ<ª)ũ("(no matching status)",Ż,ƾ
,ė,Ū,Ů,TextAlignment.LEFT);}internal static void ċ(string ĉ,l Ċ,Dictionary<string,float>ɤ,Dictionary<string,float>ɥ,List<
int>ɦ,List<string>ɧ){if(Ċ.ĵ==null||Ċ.ķ==null||Ċ.ŏ==null){ɤ.Clear();ɥ.Clear();ɦ.Clear();ɧ.Clear();return;}bool ɨ=string.
IsNullOrEmpty(ĉ);bool ɩ=string.Equals(ĉ,"OresIngots",Ī.ī);bool ɪ=string.Equals(ĉ,"Components",Ī.ī);ɤ.Clear();ɥ.Clear();ɦ.Clear();ɧ.
Clear();int ɫ=Ċ.ĵ.Length;for(int á=0;á<ɫ;á++){if(Ċ.ķ==null||Ċ.ŏ==null||Ċ.ķ[á]<=0.001f)continue;string ɬ=Ċ.ŏ[á]??"";string ɭ=Ċ
.ĵ[á]??"";if(ɬ=="Ore"){float ɮ;ɤ[ɭ]=ɤ.TryGetValue(ɭ,out ɮ)?ɮ+Ċ.ķ[á]:Ċ.ķ[á];}else if(ɬ=="Ingot"){float ɮ;ɥ[ɭ]=ɥ.
TryGetValue(ɭ,out ɮ)?ɮ+Ċ.ķ[á]:Ċ.ķ[á];}else{ɦ.Add(á);}}if(!ɪ){if(ɨ||ɩ){foreach(var ú in ɤ.Keys)ɧ.Add(ú);foreach(var ú in ɥ.Keys){if(
!ɤ.ContainsKey(ú))ɧ.Add(ú);}}else{foreach(var ú in ɤ.Keys){if(string.Equals(ú,ĉ,Ī.ī))ɧ.Add(ú);}foreach(var ú in ɥ.Keys){
if(ɤ.ContainsKey(ú))continue;if(string.Equals(ú,ĉ,Ī.ī))ɧ.Add(ú);}}ɧ.Sort(StringComparer.OrdinalIgnoreCase);}ɦ.Sort((ǁ,Ƕ)=>
string.Compare(Ċ.ĵ[ǁ]??"",Ċ.ĵ[Ƕ]??"",Ī.ī));if(ɩ)ɦ.Clear();else if(!ɨ&&!ɪ){for(int ɯ=ɦ.Count-1;ɯ>=0;ɯ--){int Ĵ=ɦ[ɯ];string ɰ=Ċ.
ĵ[Ĵ]??"";if(!string.Equals(ɰ,ĉ,Ī.ī))ɦ.RemoveAt(ɯ);}}}sealed class Ǜ:º{private readonly A ą;public Ǜ(A f){ą=f;}private
static int ɷ(string i,r s){if(s==null)return 0;if(string.IsNullOrEmpty(i))return 3;int ǖ=0;string ɲ="Batteries x"+s.ɱ;string ɴ
="Reactors x"+s.ɳ;string ɶ="Engines x"+s.ɵ;if(ɲ.IndexOf(i,Ī.ī)>=0)ǖ++;if(ɴ.IndexOf(i,Ī.ī)>=0)ǖ++;if(ɶ.IndexOf(i,Ī.ī)>=0)ǖ
++;return ǖ;}public float v(A f,VRageMath.Vector2 g,VRageMath.RectangleF h,string i,j k,l m,n o,p q,r s,t u){if(s==null)
return g.Y*0.04f;int ǭ=ɷ(i,s);return g.Y*0.035f+ǭ*(g.Y*0.11f)+g.Y*0.02f;}public void µ(A f,MySpriteDrawFrame w,VRageMath.
Vector2 g,VRageMath.RectangleF x,string i,float y,float z,float ª,j k,l m,n o,p q,r s,t u){if(s==null)return;int ǭ=ɷ(i,s);float
Ǆ=g.Y*0.035f+ǭ*(g.Y*0.11f)+g.Y*0.02f;if(y+Ǆ<=z||y>=ª)return;if(ǭ==0)return;float ļ=x.Width;float Ľ=x.X;float ǆ=Ľ+ļ*0.5f;
var ɸ=new VRageMath.Vector2(ļ,g.Y);float ɺ=s.ɹ>1e-6f?s.ɹ:1f;float ɼ=Ś.ś(s.ɻ/ɺ,0f,1f);float ɾ=s.ɽ>1e-6f?s.ɽ:1f;float ʀ=Ś.ś(s
.ɿ/ɾ,0f,1f);float ʂ=s.ʁ>1e-6f?s.ʁ:1f;float ʄ=Ś.ś(s.ʃ/ʂ,0f,1f);string ʅ="Batteries x"+s.ɱ;string ʆ="Reactors x"+s.ɳ;string
ʇ="Engines x"+s.ɵ;var Ţ=new string[ǭ];var ţ=new float[ǭ];var Ť=new string[ǭ];int Ĵ=0;if(string.IsNullOrEmpty(i)||ʅ.
IndexOf(i,Ī.ī)>=0){Ţ[Ĵ]=ʅ;ţ[Ĵ]=ɼ;Ť[Ĵ]="OUT:"+s.ɻ.ToString("0.0")+" IN:"+s.ʈ.ToString("0.0");Ĵ++;}if(string.IsNullOrEmpty(i)||ʆ.
IndexOf(i,Ī.ī)>=0){Ţ[Ĵ]=ʆ;ţ[Ĵ]=ʀ;Ť[Ĵ]="OUT:"+s.ɿ.ToString("0.0");Ĵ++;}if(string.IsNullOrEmpty(i)||ʇ.IndexOf(i,Ī.ī)>=0){Ţ[Ĵ]=ʇ;ţ
[Ĵ]=ʄ;Ť[Ĵ]="OUT:"+s.ʃ.ToString("0.0");Ĵ++;}ą.ŧ(y,ɸ,new VRageMath.Vector2(ǆ,0f),Ţ,ţ,Ť,new VRageMath.Color(255,0,0,200),
true);}}sealed class ǜ:º{private readonly A ą;public ǜ(A f){ą=f;}private static int ʏ(string i,p q){if(q==null)return 0;if(
string.IsNullOrEmpty(i))return 4;int ǖ=0;string ʉ="Total";string ʋ="Generators x"+q.ʊ;string ʍ="Irrigation x"+q.ʌ;string ʎ=
"Cargo";if(ʉ.IndexOf(i,Ī.ī)>=0)ǖ++;if(ʋ.IndexOf(i,Ī.ī)>=0)ǖ++;if(ʍ.IndexOf(i,Ī.ī)>=0)ǖ++;if(ʎ.IndexOf(i,Ī.ī)>=0)ǖ++;return ǖ;}
public float v(A f,VRageMath.Vector2 g,VRageMath.RectangleF h,string i,j k,l m,n o,p q,r s,t u){if(q==null)return g.Y*0.04f;
int ǭ=ʏ(i,q);return g.Y*0.035f+ǭ*(g.Y*0.11f)+g.Y*0.02f;}public void µ(A f,MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath
.RectangleF x,string i,float y,float z,float ª,j k,l m,n o,p q,r s,t u){if(q==null)return;int ǭ=ʏ(i,q);float Ǆ=g.Y*0.035f
+ǭ*(g.Y*0.11f)+g.Y*0.02f;if(y+Ǆ<=z||y>=ª)return;if(ǭ==0)return;float ļ=x.Width;float Ľ=x.X;float ǆ=Ľ+ļ*0.5f;var ɸ=new
VRageMath.Vector2(ļ,g.Y);string ʐ="Total";string ʑ="Generators x"+q.ʊ;string ʒ="Irrigation x"+q.ʌ;string ʓ="Cargo";var Ţ=new
string[ǭ];var ţ=new float[ǭ];var Ť=new string[ǭ];int Ĵ=0;if(string.IsNullOrEmpty(i)||ʐ.IndexOf(i,Ī.ī)>=0){Ţ[Ĵ]=ʐ;ţ[Ĵ]=q.ʔ;Ť[Ĵ]
=ĭ.Į(q.ʕ);Ĵ++;}if(string.IsNullOrEmpty(i)||ʑ.IndexOf(i,Ī.ī)>=0){Ţ[Ĵ]=ʑ;ţ[Ĵ]=q.ʖ;Ť[Ĵ]=ĭ.Į(q.ʗ);Ĵ++;}if(string.
IsNullOrEmpty(i)||ʒ.IndexOf(i,Ī.ī)>=0){Ţ[Ĵ]=ʒ;ţ[Ĵ]=q.ʘ;Ť[Ĵ]=ĭ.Į(q.ʙ);Ĵ++;}if(string.IsNullOrEmpty(i)||ʓ.IndexOf(i,Ī.ī)>=0){Ţ[Ĵ]=ʓ;ţ[Ĵ
]=q.ʚ;Ť[Ĵ]=ĭ.Į(q.ʛ);Ĵ++;}ą.ŧ(y,ɸ,new VRageMath.Vector2(ǆ,0f),Ţ,ţ,Ť,new VRageMath.Color(165,220,255,200),true);}}sealed
class ǝ:º{private readonly A ą;public ǝ(A f){ą=f;}public float v(A f,VRageMath.Vector2 g,VRageMath.RectangleF h,string i,j k,
l m,n o,p q,r s,t u){if(o==null||o.ʜ==null)return g.Y*0.04f;float ʝ=g.Y*0.072f;if(string.IsNullOrEmpty(i)){int ʞ=o.ʜ.
Length;int ʟ=ʞ>0?(ʞ+1)/2:1;return g.Y*0.180f+ʟ*ʝ+g.Y*0.02f;}if(string.Equals(i,"Priority",Ī.ī))return g.Y*0.180f;int ʠ=0;int ǖ
=o.ʜ.Length;for(int á=0;á<ǖ;á++){string ɰ=o.ʜ[á]??"";if(ɰ.IndexOf(i,Ī.ī)>=0)ʠ++;}int ʡ=ʠ>0?(ʠ+1)/2:0;return g.Y*0.08f+ʡ*ʝ
+g.Y*0.02f;}public void µ(A f,MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,string i,float y,float z,
float ª,j k,l m,n Ą,p q,r s,t u){if(Ą==null||Ą.ʜ==null)return;float ʝ=g.Y*0.072f;float ʢ;if(string.IsNullOrEmpty(i)){int ʣ=Ą.
ʜ.Length;int ʟ=ʣ>0?(ʣ+1)/2:1;ʢ=g.Y*0.180f+ʟ*ʝ+g.Y*0.02f;}else if(string.Equals(i,"Priority",Ī.ī))ʢ=g.Y*0.180f;else{int ʤ=
0;for(int ɯ=0;ɯ<Ą.ʜ.Length;ɯ++){if((Ą.ʜ[ɯ]??"").IndexOf(i,Ī.ī)>=0)ʤ++;}int ʡ=ʤ>0?(ʤ+1)/2:0;ʢ=g.Y*0.08f+ʡ*ʝ+g.Y*0.02f;}if(
y+ʢ<=z||y>=ª)return;float ļ=x.Width;float Ľ=x.X;float ǆ=Ľ+ļ*0.5f;float ʥ=ļ*0.5f;const float ʦ=0.52f;const float ʧ=0.58f;
float ʨ=g.Y*0.038f;if(string.Equals(i,"Priority",Ī.ī)){string ʪ=Ą.ʩ;string ʬ=Ą.ʫ;if(string.IsNullOrEmpty(ʪ)){ʪ=
"1. Fe  2. Co  3. Ni";ʬ=null;}ą.ũ(ʪ,ǆ,y+g.Y*0.025f,0.72f,A.ŭ,A.ū,TextAlignment.CENTER);if(!string.IsNullOrEmpty(ʬ))ą.ũ(ʬ,ǆ,y+g.Y*0.075f,0.72f
,A.ŭ,A.ū,TextAlignment.CENTER);return;}if(string.IsNullOrEmpty(i)){string ʪ=Ą.ʩ;string ʬ=Ą.ʫ;if(string.IsNullOrEmpty(ʪ)){
ʪ="1. Fe  2. Co  3. Ni";ʬ=null;}ą.ũ(ʪ,ǆ,y+g.Y*0.025f,0.72f,A.ŭ,A.ū,TextAlignment.CENTER);if(!string.IsNullOrEmpty(ʬ))ą.ũ(
ʬ,ǆ,y+g.Y*0.075f,0.72f,A.ŭ,A.ū,TextAlignment.CENTER);}float ʭ=string.IsNullOrEmpty(i)?y+g.Y*0.180f:y+g.Y*0.08f;int ʞ=Ą.ʜ.
Length;int ʮ=0;for(int á=0;á<ʞ;á++){if(!string.IsNullOrEmpty(i)){string ʯ=Ą.ʜ[á]??"";if(ʯ.IndexOf(i,Ī.ī)<0)continue;}int ʰ=ʮ%2
;int ʱ=ʮ/2;ʮ++;float ʲ=Ľ+ʰ*ʥ;float ʳ=ʭ+ʱ*ʝ;float ʴ=ʳ-g.Y*0.018f;float ʵ=ʲ+ʥ*0.065f;string ʶ=Ą.ʜ[á]??"Unknown Refinery";
bool ʷ=(Ą.ʷ!=null&&á<Ą.ʷ.Length)?Ą.ʷ[á]:false;bool ʸ=(Ą.ʸ!=null&&á<Ą.ʸ.Length)?Ą.ʸ[á]:false;string ʺ=(Ą.ʹ!=null&&á<Ą.ʹ.
Length)?Ą.ʹ[á]:"";var ʻ=A.Ū;if(ʷ)ʻ=A.Ɓ;else if(ʸ)ʻ=A.ƀ;string ʽ=ʸ&&!string.IsNullOrEmpty(ʺ)?ĭ.ʼ(ʺ):"-";ą.ũ(ʽ,ʲ+ʥ*0.24f,ʴ,ʦ,new
VRageMath.Color(220,220,220,255),A.Ů,TextAlignment.CENTER);ą.ũ(ʶ,ʲ+ʥ*0.36f,ʴ,ʧ,A.ŭ,A.ū,TextAlignment.LEFT);ą.ǅ("Circle",ʵ,ʳ,ʨ,ʨ,ʻ
);}}}sealed class Ǟ:º{private readonly A ą;public Ǟ(A f){ą=f;}public float v(A f,VRageMath.Vector2 g,VRageMath.RectangleF
h,string i,j k,l m,n o,p q,r s,t u){if(u==null||u.ʾ)return g.Y*0.22f;int ɋ=0;if(u.ʿ)ɋ++;if(u.ˀ)ɋ++;if(u.ˁ)ɋ++;if(u.ˆ)ɋ++;
if(u.ˇ)ɋ++;if(u.ˈ)ɋ++;if(ɋ==0)ɋ=1;return ɋ*(g.Y*0.065f)+g.Y*0.02f;}public void µ(A f,MySpriteDrawFrame w,VRageMath.Vector2
g,VRageMath.RectangleF x,string i,float y,float z,float ª,j k,l m,n o,p q,r s,t u){int ɋ=0;if(u!=null&&!u.ʾ){if(u.ʿ)ɋ++;
if(u.ˀ)ɋ++;if(u.ˁ)ɋ++;if(u.ˆ)ɋ++;if(u.ˇ)ɋ++;if(u.ˈ)ɋ++;if(ɋ==0)ɋ=1;}float ʢ=u==null||u.ʾ?g.Y*0.22f:ɋ*(g.Y*0.065f)+g.Y*
0.02f;if(y+ʢ<=z||y>=ª)return;if(u==null)return;float ļ=x.Width;float Ľ=x.X;float ǆ=Ľ+ļ*0.5f;float Ƀ=Math.Min(1f,ļ/350f);if(u.
ʾ){ą.ũ("ALL SYSTEMS NOMINAL",ǆ,y+g.Y*0.13f,1.0f*Ƀ,A.Ɓ,A.ū,TextAlignment.CENTER);return;}ą.ƣ.Clear();if(u.ʿ)ą.ƣ.Add(
"LOW POWER");if(u.ˀ)ą.ƣ.Add("CARGO FULL");if(u.ˁ)ą.ƣ.Add("LOW ICE");if(u.ˆ)ą.ƣ.Add("REFINERY STALLED");if(u.ˇ)ą.ƣ.Add(
"ASSEMBLER STALLED");if(u.ˈ)ą.ƣ.Add("NO REFINERIES");float ƾ=y+g.Y*0.02f;float ˉ=g.Y*0.065f;for(int á=0;á<ą.ƣ.Count;á++){string ǃ=ą.ƣ[á];ą.
ũ(ǃ,ǆ,ƾ,0.92f*Ƀ,A.ƀ,A.ū,TextAlignment.CENTER);ƾ+=ˉ;}}}sealed class ǟ:º{private readonly A ą;private const float ˊ=0.52f;
public ǟ(A f){ą=f;}public float v(A f,VRageMath.Vector2 g,VRageMath.RectangleF h,string i,j k,l m,n o,p q,r s,t u){return ą.ɢ(
g,h,i??"",ˊ);}public void µ(A f,MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,string i,float y,float z,
float ª,j k,l m,n o,p q,r s,t u){float ʢ=ą.ɢ(g,x,i??"",ˊ);if(y+ʢ<=z||y>=ª)return;ą.ɣ(g,x,i??"",y,z,ª,ˊ);}}}public sealed
class Ơ{public const string Ǥ="COL";float ˋ,ˌ;float ˍ,ˎ,ˏ,ː;int ˑ;public VRageMath.RectangleF Ȭ{get;private set;}public void
ȡ(float ˠ,float ˡ){ˋ=ˠ;ˍ=ˡ;ˌ=0f;ˎ=0f;ˏ=0f;ː=0f;ˑ=0;Ȭ=new VRageMath.RectangleF(0f,0f,ˋ,ˍ);}public float Ȫ{get{if(ˑ==0)
return ˌ;return Math.Max(ˌ,ˎ+Math.Max(ˏ,ː));}}public float ȸ{get{if(ˑ==0)return ˌ;if(ˑ==1)return ˎ+ˏ;return ˎ+ː;}}public float
ȼ{get{return Ȭ.X+Ȭ.Width*0.5f;}}public void ȣ(string ˢ){string ǁ=ˢ==null?"":ˢ.Trim();if(ǁ.Length==0){ˣ();return;}if(
string.Equals(ǁ,"FULL",Ī.ī)){ˤ();return;}if(string.Equals(ǁ,"LEFT",Ī.ī)){ˬ();return;}if(string.Equals(ǁ,"RIGHT",Ī.ī)){ˮ();
return;}}void ˣ(){if(ˑ==0||ˑ==2){Ͱ();ˎ=ˌ;ˏ=0f;ː=0f;ˑ=1;ͱ(1);return;}if(ˑ==1){ˑ=2;ͱ(2);}}void ˬ(){if(ˑ==1||ˑ==2)Ͱ();ˎ=ˌ;ˏ=0f;ː=
0f;ˑ=1;ͱ(1);}void ˮ(){if(ˑ==0){ˎ=ˌ;ˏ=0f;ː=0f;}else if(ˑ==1){}else if(ˑ==2){Ͱ();ˎ=ˌ;ˏ=0f;ː=0f;}ˑ=2;ͱ(2);}public void ˤ(){Ͱ(
);ˑ=0;Ȭ=new VRageMath.RectangleF(0f,0f,ˋ,ˍ);}public void Ȩ(float Ͳ){if(ˑ==0){ˌ+=Ͳ;return;}if(ˑ==1)ˏ+=Ͳ;else ː+=Ͳ;}public
void ȩ(){Ͱ();}void Ͱ(){if(ˑ==0)return;float ͳ=ˎ+Math.Max(ˏ,ː);if(ͳ>ˌ)ˌ=ͳ;ˑ=0;ˏ=0f;ː=0f;Ȭ=new VRageMath.RectangleF(0f,0f,ˋ,ˍ)
;}void ͱ(int ʰ){float ʹ=ˋ*0.5f;if(ʰ==1)Ȭ=new VRageMath.RectangleF(0f,0f,ʹ,ˍ);else Ȭ=new VRageMath.RectangleF(ʹ,0f,ʹ,ˍ);}}
public class C{}public class p{public float ʕ,ʗ,ʙ,ʛ,ʔ,ʖ,ʘ,ʚ;public int ʊ,ʌ;public bool ˁ;}public class l{public string[]ĵ,ŏ;
public float[]ķ;}public class j{public float Ͷ,ͷ,ͺ,ͻ,ͼ,ͽ,Ά,Έ,Ή,Ί,Ό,Ύ,Ώ,ΐ,Α,Β,Γ,Δ,Ε,Ζ,Η,Θ,Ι,Κ,Ŝ,ř,ş;}public class r{public
float Λ,Μ,ʈ,ɻ,Ν,ɹ,ɿ,ʃ,ɽ,ʁ;public int ɱ,ɳ,ɵ;public bool ʿ;}public class n{public string[]ʜ,ʹ,Ξ;public float[]Ο,Π;public bool[]
ʷ,ʸ;public string ʩ,ʫ;}public class t{public bool ˁ,ʿ,ˀ,ˈ,ˆ,ˇ,ʾ;public int Ρ;public string Σ;}public static class Ò{
public const string Ù="SYS_STATUS",Ú="PB1_WARNINGS",Ó="PB1ToPB2_InventorySummary",Ô="PB1ToPB2_RefineryStatus",Õ=
"PB1ToPB2_IceStatus",Ö="PB1ToPB2_PowerStatus",Ø="PB1ToPB2_InventoryDynamic";}public static class μ{private const string Τ="1";public static
ǵ ă<ǵ>(string ò){try{if(typeof(ǵ)==typeof(j))return(ǵ)(object)Υ(ò);if(typeof(ǵ)==typeof(n))return(ǵ)(object)Φ(ò);if(
typeof(ǵ)==typeof(p))return(ǵ)(object)Χ(ò);if(typeof(ǵ)==typeof(r))return(ǵ)(object)Ψ(ò);if(typeof(ǵ)==typeof(l))return(ǵ)(
object)Ω(ò);if(typeof(ǵ)==typeof(t))return(ǵ)(object)Ϊ(ò);}catch{}return default(ǵ);}private static j Υ(string ò){j ǂ=new j();
if(string.IsNullOrEmpty(ò))return ǂ;string[]Ʒ=ò.Split(';');if(Ʒ.Length==0||Ʒ[0]!=Τ)return new j();if(Ʒ.Length>1)float.
TryParse(Ʒ[1],out ǂ.Ͷ);if(Ʒ.Length>2)float.TryParse(Ʒ[2],out ǂ.ͷ);if(Ʒ.Length>3)float.TryParse(Ʒ[3],out ǂ.ͺ);if(Ʒ.Length>4)float
.TryParse(Ʒ[4],out ǂ.ͻ);if(Ʒ.Length>5)float.TryParse(Ʒ[5],out ǂ.ͼ);if(Ʒ.Length>6)float.TryParse(Ʒ[6],out ǂ.ͽ);if(Ʒ.Length
>7)float.TryParse(Ʒ[7],out ǂ.Ά);if(Ʒ.Length>8)float.TryParse(Ʒ[8],out ǂ.Έ);if(Ʒ.Length>9)float.TryParse(Ʒ[9],out ǂ.Ή);if(
Ʒ.Length>10)float.TryParse(Ʒ[10],out ǂ.Ί);if(Ʒ.Length>11)float.TryParse(Ʒ[11],out ǂ.Ό);if(Ʒ.Length>12)float.TryParse(Ʒ[12
],out ǂ.Ύ);if(Ʒ.Length>13)float.TryParse(Ʒ[13],out ǂ.Ώ);if(Ʒ.Length>14)float.TryParse(Ʒ[14],out ǂ.ΐ);if(Ʒ.Length>15)float
.TryParse(Ʒ[15],out ǂ.Α);if(Ʒ.Length>16)float.TryParse(Ʒ[16],out ǂ.Β);if(Ʒ.Length>17)float.TryParse(Ʒ[17],out ǂ.Γ);if(Ʒ.
Length>18)float.TryParse(Ʒ[18],out ǂ.Δ);if(Ʒ.Length>19)float.TryParse(Ʒ[19],out ǂ.Ε);if(Ʒ.Length>20)float.TryParse(Ʒ[20],out ǂ
.Ζ);if(Ʒ.Length>21)float.TryParse(Ʒ[21],out ǂ.Η);if(Ʒ.Length>22)float.TryParse(Ʒ[22],out ǂ.Θ);if(Ʒ.Length>23)float.
TryParse(Ʒ[23],out ǂ.Ι);if(Ʒ.Length>24)float.TryParse(Ʒ[24],out ǂ.Κ);if(Ʒ.Length>25)float.TryParse(Ʒ[25],out ǂ.Ŝ);if(Ʒ.Length>26
)float.TryParse(Ʒ[26],out ǂ.ř);if(Ʒ.Length>27)float.TryParse(Ʒ[27],out ǂ.ş);return ǂ;}private static n Φ(string ò){n ǂ=
new n();if(string.IsNullOrEmpty(ò))return ǂ;string[]Ʒ=ò.Split(';');if(Ʒ.Length==0||Ʒ[0]!=Τ)return new n();if(Ʒ.Length>1)ǂ.ʜ
=Ϋ(Ʒ[1]);if(Ʒ.Length>2)ǂ.ʹ=Ϋ(Ʒ[2]);if(Ʒ.Length>3)ǂ.Ο=ά(Ʒ[3]);if(Ʒ.Length>4)ǂ.Ξ=Ϋ(Ʒ[4]);if(Ʒ.Length>5)ǂ.Π=ά(Ʒ[5]);if(Ʒ.
Length>6)ǂ.ʷ=έ(Ʒ[6]);if(Ʒ.Length>7)ǂ.ʸ=έ(Ʒ[7]);if(Ʒ.Length>8)ǂ.ʩ=Ʒ[8];if(Ʒ.Length>9)ǂ.ʫ=Ʒ[9];return ǂ;}private static p Χ(
string ò){p ǂ=new p();if(string.IsNullOrEmpty(ò))return ǂ;string[]Ʒ=ò.Split(';');if(Ʒ.Length==0||Ʒ[0]!=Τ)return new p();if(Ʒ.
Length>1)float.TryParse(Ʒ[1],out ǂ.ʕ);if(Ʒ.Length>2)float.TryParse(Ʒ[2],out ǂ.ʗ);if(Ʒ.Length>3)float.TryParse(Ʒ[3],out ǂ.ʙ);if
(Ʒ.Length>4)float.TryParse(Ʒ[4],out ǂ.ʛ);if(Ʒ.Length>5)float.TryParse(Ʒ[5],out ǂ.ʔ);if(Ʒ.Length>6)float.TryParse(Ʒ[6],out
ǂ.ʖ);if(Ʒ.Length>7)float.TryParse(Ʒ[7],out ǂ.ʘ);if(Ʒ.Length>8)float.TryParse(Ʒ[8],out ǂ.ʚ);int ή;if(Ʒ.Length>9&&int.
TryParse(Ʒ[9],out ή))ǂ.ʊ=ή;if(Ʒ.Length>10&&int.TryParse(Ʒ[10],out ή))ǂ.ʌ=ή;if(Ʒ.Length>11)ǂ.ˁ=ί(Ʒ[11]);return ǂ;}private static
r Ψ(string ò){r ǂ=new r();if(string.IsNullOrEmpty(ò))return ǂ;string[]Ʒ=ò.Split(';');if(Ʒ.Length==0||Ʒ[0]!=Τ)return new r
();if(Ʒ.Length>1)float.TryParse(Ʒ[1],out ǂ.Λ);if(Ʒ.Length>2)float.TryParse(Ʒ[2],out ǂ.Μ);if(Ʒ.Length>3)float.TryParse(Ʒ[3
],out ǂ.ʈ);if(Ʒ.Length>4)float.TryParse(Ʒ[4],out ǂ.ɻ);if(Ʒ.Length>5)float.TryParse(Ʒ[5],out ǂ.Ν);if(Ʒ.Length>6)float.
TryParse(Ʒ[6],out ǂ.ɹ);if(Ʒ.Length>7)float.TryParse(Ʒ[7],out ǂ.ɽ);if(Ʒ.Length>8)float.TryParse(Ʒ[8],out ǂ.ʁ);if(Ʒ.Length>9)float
.TryParse(Ʒ[9],out ǂ.ɿ);if(Ʒ.Length>10)float.TryParse(Ʒ[10],out ǂ.ʃ);int ΰ;if(Ʒ.Length>11&&int.TryParse(Ʒ[11],out ΰ))ǂ.ɱ=
ΰ;if(Ʒ.Length>12&&int.TryParse(Ʒ[12],out ΰ))ǂ.ɳ=ΰ;if(Ʒ.Length>13&&int.TryParse(Ʒ[13],out ΰ))ǂ.ɵ=ΰ;if(Ʒ.Length>14)ǂ.ʿ=ί(Ʒ[
14]);return ǂ;}private static l Ω(string ò){l ǂ=new l();if(string.IsNullOrEmpty(ò))return ǂ;string[]Ʒ=ò.Split(';');if(Ʒ.
Length==0||Ʒ[0]!=Τ)return new l();if(Ʒ.Length>1)ǂ.ĵ=Ϋ(Ʒ[1]);if(Ʒ.Length>2)ǂ.ķ=ά(Ʒ[2]);if(Ʒ.Length>3)ǂ.ŏ=Ϋ(Ʒ[3]);return ǂ;}
private static t Ϊ(string ò){t ǂ=new t();if(string.IsNullOrEmpty(ò))return ǂ;string[]Ʒ=ò.Split(';');if(Ʒ.Length==0||Ʒ[0]!=Τ)
return new t();if(Ʒ.Length>1)ǂ.ˁ=ί(Ʒ[1]);if(Ʒ.Length>2)ǂ.ʿ=ί(Ʒ[2]);if(Ʒ.Length>3)ǂ.ˀ=ί(Ʒ[3]);if(Ʒ.Length>4)ǂ.ˈ=ί(Ʒ[4]);if(Ʒ.
Length>5)ǂ.ˆ=ί(Ʒ[5]);if(Ʒ.Length>6)ǂ.ˇ=ί(Ʒ[6]);if(Ʒ.Length>7){int α;if(int.TryParse(Ʒ[7],out α))ǂ.Ρ=α;}if(Ʒ.Length>8)ǂ.Σ=Ʒ[8];
if(Ʒ.Length>9)ǂ.ʾ=ί(Ʒ[9]);return ǂ;}private static bool ί(string ƿ){if(string.IsNullOrEmpty(ƿ))return false;if(ƿ[0]=='1'&&
ƿ.Length==1)return true;if(ƿ.Length==4&&(ƿ[0]=='t'||ƿ[0]=='T')&&(ƿ[1]=='r'||ƿ[1]=='R')&&(ƿ[2]=='u'||ƿ[2]=='U')&&(ƿ[3]==
'e'||ƿ[3]=='E'))return true;return false;}private static string[]Ϋ(string ƿ){if(ƿ==null||ƿ.Length==0)return new string[0];
int ǖ=β(ƿ);string[]γ=new string[ǖ];δ(ƿ,γ);return γ;}private static float[]ά(string ƿ){if(ƿ==null||ƿ.Length==0)return new
float[0];int ǖ=ε(ƿ);float[]ζ=new float[ǖ];int ɡ=0;int η=0;for(int á=0;á<=ƿ.Length;á++){if(á==ƿ.Length||ƿ[á]=='|'){int à=á-η;
string θ=à>0?ƿ.Substring(η,à):string.Empty;float.TryParse(θ,out ζ[ɡ]);ɡ++;η=á+1;}}return ζ;}private static bool[]έ(string ƿ){
if(ƿ==null||ƿ.Length==0)return new bool[0];int ǖ=ε(ƿ);bool[]ζ=new bool[ǖ];int ɡ=0;int η=0;for(int á=0;á<=ƿ.Length;á++){if(
á==ƿ.Length||ƿ[á]=='|'){int à=á-η;string θ=à>0?ƿ.Substring(η,à):string.Empty;ζ[ɡ]=ί(θ);ɡ++;η=á+1;}}return ζ;}private
static int β(string ƿ){int ǭ=1;for(int á=0;á<ƿ.Length;á++){if(ƿ[á]=='\\'&&á+1<ƿ.Length){á++;continue;}if(ƿ[á]=='|')ǭ++;}return
ǭ;}private static void δ(string ƿ,string[]ι){StringBuilder κ=new StringBuilder(32);int λ=0;int á=0;while(á<ƿ.Length){char
ĳ=ƿ[á];if(ĳ=='\\'&&á+1<ƿ.Length){char ǖ=ƿ[á+1];if(ǖ=='\\'||ǖ=='|')κ.Append(ǖ);else{κ.Append('\\');κ.Append(ǖ);}á+=2;}else
if(ĳ=='|'){ι[λ++]=κ.ToString();κ.Length=0;á++;}else{κ.Append(ĳ);á++;}}ι[λ++]=κ.ToString();}private static int ε(string ƿ){
int ǭ=1;for(int á=0;á<ƿ.Length;á++){if(ƿ[á]=='|')ǭ++;}return ǭ;}}public static class Ă{public static ǵ ă<ǵ>(string ò){try{
if(ò==null)return default(ǵ);return μ.ă<ǵ>(ò);}catch{}return default(ǵ);}}public static class Ā{private const uint ν=
2166136261u,ξ=16777619u;public static bool ā(string ο,string π,Dictionary<string,long>ρ,out string T,out string ς){T=null;ς=null;if
(ο==null||ρ==null){return false;}string[]γ=ο.Split(new[]{'|'},4);if(γ.Length!=4){return false;}string σ=γ[0];string τ=γ[1
];string υ=γ[2];string φ=γ[3];if(σ==null||τ==null||υ==null||φ==null){return false;}string χ;if(υ.Length==0){χ="";}else{
byte[]Û;try{Û=Convert.FromBase64String(υ);}catch{return false;}χ=Encoding.UTF8.GetString(Û);}long ψ;if(!long.TryParse(τ,out
ψ)){return false;}long ω=0;long ɮ;if(ρ.TryGetValue(σ,out ɮ)){ω=ɮ;}if(ψ<=ω){return false;}string ϊ=π??"";uint Ǆ=ν;Ǆ=ϋ(Ǆ,σ)
;Ǆ=ϋ(Ǆ,τ);Ǆ=ϋ(Ǆ,χ);Ǆ=ϋ(Ǆ,ϊ);string ό=Ǆ.ToString("X8");if(!string.Equals(φ,ό,StringComparison.Ordinal)){return false;}ρ[σ]
=ψ;T=σ;ς=χ;return true;}private static uint ϋ(uint ύ,string ƿ){if(ƿ==null||ƿ.Length==0){return ύ;}for(int á=0;á<ƿ.Length;
á++){char ĳ=ƿ[á];ύ^=(byte)(ĳ&0xFF);ύ*=ξ;ύ^=(byte)((ĳ>>8)&0xFF);ύ*=ξ;}return ύ;}}public static class ȋ{public static bool
Ȍ(string Ķ,string ü){if(string.IsNullOrEmpty(Ķ)||string.IsNullOrEmpty(ü))return false;return Ķ.IndexOf(ü,StringComparison
.OrdinalIgnoreCase)>=0;}}public static class ĭ{private static readonly StringBuilder ώ=new StringBuilder(48);public
static string Ş(float Ϗ){if(float.IsNaN(Ϗ))return"NaN%";if(float.IsInfinity(Ϗ))return Ϗ>0f?"Infinity%":"-Infinity%";int Ʒ=(int
)Math.Round((double)Ϗ);ώ.Clear();ώ.Append(Ʒ.ToString());ώ.Append('%');return ώ.ToString();}public static string Į(float Ϗ
){if(float.IsNaN(Ϗ))return"NaN";if(float.IsInfinity(Ϗ))return Ϗ>0f?"Infinity":"-Infinity";bool ϐ=Ϗ<0f;double ϑ=ϐ?-(double
)Ϗ:(double)Ϗ;string æ="";double ϒ=1.0;if(ϑ>=1e9){æ="B";ϒ=1e9;}else if(ϑ>=1e6){æ="M";ϒ=1e6;}else if(ϑ>=1e3){æ="k";ϒ=1e3;}ώ
.Clear();if(ϐ)ώ.Append('-');if(æ.Length>0){double ɉ=ϑ/ϒ;ɉ=Math.Round(ɉ*10.0)/10.0;ώ.Append(ɉ.ToString("0.0"));ώ.Append(æ)
;}else{float ϓ=ϐ?-(float)ϑ:(float)ϑ;ώ.Append(ϓ.ToString("0.######"));}return ώ.ToString();}public static string ʼ(string
ϔ){if(string.IsNullOrEmpty(ϔ)){return"-";}if(string.Equals(ϔ,"Iron",Ī.ī)){return"Fe";}if(string.Equals(ϔ,"Nickel",Ī.ī)){
return"Ni";}if(string.Equals(ϔ,"Cobalt",Ī.ī)){return"Co";}if(string.Equals(ϔ,"Silicon",Ī.ī)){return"Si";}if(string.Equals(ϔ,
"Silver",Ī.ī)){return"Ag";}if(string.Equals(ϔ,"Gold",Ī.ī)){return"Au";}if(string.Equals(ϔ,"Magnesium",Ī.ī)){return"Mg";}if(
string.Equals(ϔ,"Platinum",Ī.ī)){return"Pt";}if(string.Equals(ϔ,"Uranium",Ī.ī)){return"U";}if(string.Equals(ϔ,"Stone",Ī.ī)){
return"St";}if(string.Equals(ϔ,"Ice",Ī.ī)){return"Ic";}if(ϔ.Length<=2){return ϔ.ToUpperInvariant();}return ϔ.Substring(0,2).
ToUpperInvariant();}}public static class Ś{public static float ś(float Ϗ,float ϕ,float š){if(ϕ>š){float ϖ=ϕ;ϕ=š;š=ϖ;}if(Ϗ<ϕ)return ϕ;if(
Ϗ>š)return š;return Ϗ;}}public static class Ī{public const StringComparison ī=StringComparison.OrdinalIgnoreCase;