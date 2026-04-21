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
string,long>Å=new Dictionary<string,long>(),Æ=new Dictionary<string,long>();private readonly List<string>Ç=new List<string>();
j È=new j();n É=new n();p Ê=new p();r Ë=new r();l Ì=new l();t Í=new t();private readonly Dictionary<long,string>Î=new
Dictionary<long,string>();private readonly Dictionary<long,long>Ï=new Dictionary<long,long>();private readonly List<long>Ð=new
List<long>();public j Y{get{return È;}}public n Z{get{return É;}}public p a{get{return Ê;}}public r b{get{return Ë;}}public
l c{get{return Ì;}}public t d{get{return Í;}}public long R{get;private set;}public bool P{get;private set;}public string
S{get;private set;}="";public string V{get;set;}public void H(MyGridProgram Ñ){À=Ñ;Ã=0;Ò();Ó(Ô.Õ);Ó(Ô.Ö);Ó(Ô.Ø);Ó(Ô.Ù);Ó(
Ô.Ú);Ó(Ô.Û);Ó(Ô.Ü);}private static string å(string Ý,string Þ){if(string.IsNullOrEmpty(Ý))return Þ;int ß=Ý.IndexOf('-');
string à=ß<0?Ý:Ý.Substring(0,ß);char[]á=new char[3];int â=0;for(int ã=0;ã<à.Length&&â<3;ã++){char ä=à[ã];if(char.
IsLetterOrDigit(ä)){á[â]=char.ToUpperInvariant(ä);â++;}}if(â==0)return Þ;return new string(á,0,â);}string ê(string æ,string ç){string è
=À.Me.EntityId.ToString("X");è=è.Substring(Math.Max(0,è.Length-4));string é=å(æ,ç);return é+"-"+è;}void Ò(){
IMyProgrammableBlock ë=À.Me;var ì=new VRage.Game.ModAPI.Ingame.Utilities.MyIni();VRage.Game.ModAPI.Ingame.Utilities.MyIniParseResult í;if(!ì
.TryParse(ë.CustomData??"",out í)){ì.Clear();}string î=ì.Get("Network","SharedKey").ToString("");bool ï=ì.Get("Network",
"EnableNetwork").ToBoolean(true);string ð=ì.Get("Network","PBID").ToString("");if(ð!=null)ð=ð.Trim();this.S=ê(ð??"","DIS");if(ì.
ContainsKey("Network","SenderId"))ì.Delete("Network","SenderId");ì.Set("Network","EnableNetwork",ï);ì.SetComment("Network",
"EnableNetwork","See docs/configuration.md — set false for offline mode (no envelope parse).");ì.Set("Network","PBID",this.S);ì.
SetComment("Network","PBID","Format: ABC-XXXX. You may change the 3-letter prefix. The 4-character suffix is locked to this block's ID and will auto-reset if changed."
);ì.Set("Network","SharedKey",î);ì.SetComment("Network","SharedKey","Must match PB1 SharedKey.");ë.CustomData=ì.ToString(
);P=ï;Á=î==null?"":î.Trim();}void Ó(string ñ){IMyBroadcastListener ò=À.IGC.RegisterBroadcastListener(ñ);ò.
SetMessageCallback("PB1_MSG");Â[Ã]=ò;Ã++;}public void e(){long ó=System.DateTime.UtcNow.Ticks;ô.õ(Å,Æ,ó,ô.ö,Ç);for(int ã=0;ã<Ã;ã++){
IMyBroadcastListener ò=Â[ã];while(ò.HasPendingMessage){MyIGCMessage ø=ò.AcceptMessage();object ù=ø.Data;string ú=ù as string;if(ú==null)
continue;try{û(ø,ú,ó);}catch{}}}}public void ā(List<string>ü){ü.Clear();long ý=System.DateTime.UtcNow.Ticks-30L*System.TimeSpan.
TicksPerSecond;Ð.Clear();foreach(KeyValuePair<long,string>ÿ in Î){long þ;if(!Ï.TryGetValue(ÿ.Key,out þ)||þ<ý)Ð.Add(ÿ.Key);}for(int ã=0
;ã<Ð.Count;ã++){long Ā=Ð[ã];Î.Remove(Ā);Ï.Remove(Ā);}foreach(KeyValuePair<long,string>ÿ in Î)ü.Add(ÿ.Value);if(!string.
IsNullOrEmpty(V))ü.Add(V);}void û(MyIGCMessage ø,string ú,long Ă){if(string.IsNullOrEmpty(Á)){return;}string ă=ø.Tag;if(ă==Ô.Û){long
Ą=ø.Source;Ï[Ą]=Ă;Î[Ą]=ú??string.Empty;return;}string ą;string Ć;if(!ô.ć(ú,Á,Å,Æ,Ă,ô.ö,out Ć,out ą)){Ä++;return;}if(ă==Ô.
Õ){R=System.DateTime.UtcNow.Ticks;j Ċ=Ĉ.ĉ<j>(ą);if(Ċ!=null)È=Ċ;return;}if(ă==Ô.Ö){R=System.DateTime.UtcNow.Ticks;n Ċ=Ĉ.ĉ<
n>(ą);if(Ċ!=null)É=Ċ;return;}if(ă==Ô.Ø){R=System.DateTime.UtcNow.Ticks;p Ċ=Ĉ.ĉ<p>(ą);if(Ċ!=null)Ê=Ċ;return;}if(ă==Ô.Ù){R=
System.DateTime.UtcNow.Ticks;r Ċ=Ĉ.ĉ<r>(ą);if(Ċ!=null)Ë=Ċ;return;}if(ă==Ô.Ú){R=System.DateTime.UtcNow.Ticks;l Ċ=Ĉ.ĉ<l>(ą);if(Ċ
!=null)Ì=Ċ;return;}if(ă==Ô.Ü){R=System.DateTime.UtcNow.Ticks;t Ċ=Ĉ.ĉ<t>(ą);if(Ċ!=null)Í=Ċ;return;}}}public static class Ų{
public static ċ ĉ<ċ>(string ù){try{if(typeof(ċ)==typeof(j))return(ċ)(object)Č(ù);if(typeof(ċ)==typeof(n))return(ċ)(object)č(ù)
;if(typeof(ċ)==typeof(p))return(ċ)(object)Ď(ù);if(typeof(ċ)==typeof(r))return(ċ)(object)ď(ù);if(typeof(ċ)==typeof(l))
return(ċ)(object)Đ(ù);if(typeof(ċ)==typeof(t))return(ċ)(object)đ(ù);}catch{}return default(ċ);}private static j Č(string ù){j
Ē=new j();if(string.IsNullOrEmpty(ù))return Ē;string[]ē=ù.Split(';');if(ē.Length==0||ē[0]!=Ĕ)return new j();if(ē.Length>1
)float.TryParse(ē[1],out Ē.ĕ);if(ē.Length>2)float.TryParse(ē[2],out Ē.Ė);if(ē.Length>3)float.TryParse(ē[3],out Ē.ė);if(ē.
Length>4)float.TryParse(ē[4],out Ē.Ę);if(ē.Length>5)float.TryParse(ē[5],out Ē.ę);if(ē.Length>6)float.TryParse(ē[6],out Ē.Ě);if
(ē.Length>7)float.TryParse(ē[7],out Ē.ě);if(ē.Length>8)float.TryParse(ē[8],out Ē.Ĝ);if(ē.Length>9)float.TryParse(ē[9],out
Ē.ĝ);if(ē.Length>10)float.TryParse(ē[10],out Ē.Ğ);if(ē.Length>11)float.TryParse(ē[11],out Ē.ğ);if(ē.Length>12)float.
TryParse(ē[12],out Ē.Ġ);if(ē.Length>13)float.TryParse(ē[13],out Ē.ġ);if(ē.Length>14)float.TryParse(ē[14],out Ē.Ģ);if(ē.Length>15
)float.TryParse(ē[15],out Ē.ģ);if(ē.Length>16)float.TryParse(ē[16],out Ē.Ĥ);if(ē.Length>17)float.TryParse(ē[17],out Ē.ĥ);
if(ē.Length>18)float.TryParse(ē[18],out Ē.Ħ);if(ē.Length>19)float.TryParse(ē[19],out Ē.ħ);if(ē.Length>20)float.TryParse(ē[
20],out Ē.Ĩ);if(ē.Length>21)float.TryParse(ē[21],out Ē.ĩ);if(ē.Length>22)float.TryParse(ē[22],out Ē.Ī);if(ē.Length>23)
float.TryParse(ē[23],out Ē.ī);if(ē.Length>24)float.TryParse(ē[24],out Ē.Ĭ);if(ē.Length>25)float.TryParse(ē[25],out Ē.ĭ);if(ē.
Length>26)float.TryParse(ē[26],out Ē.Į);if(ē.Length>27)float.TryParse(ē[27],out Ē.į);return Ē;}private static n č(string ù){n
Ē=new n();if(string.IsNullOrEmpty(ù))return Ē;string[]ē=ù.Split(';');if(ē.Length==0||ē[0]!=Ĕ)return new n();if(ē.Length>1
)Ē.İ=ı(ē[1]);if(ē.Length>2)Ē.Ĳ=ı(ē[2]);if(ē.Length>3)Ē.ĳ=Ĵ(ē[3]);if(ē.Length>4)Ē.ĵ=ı(ē[4]);if(ē.Length>5)Ē.Ķ=Ĵ(ē[5]);if(ē
.Length>6)Ē.ķ=ĸ(ē[6]);if(ē.Length>7)Ē.Ĺ=ĸ(ē[7]);if(ē.Length>8)Ē.ĺ=ē[8];if(ē.Length>9)Ē.Ļ=ē[9];return Ē;}private static p
Ď(string ù){p Ē=new p();if(string.IsNullOrEmpty(ù))return Ē;string[]ē=ù.Split(';');if(ē.Length==0||ē[0]!=Ĕ)return new p()
;if(ē.Length>1)float.TryParse(ē[1],out Ē.ļ);if(ē.Length>2)float.TryParse(ē[2],out Ē.Ľ);if(ē.Length>3)float.TryParse(ē[3],
out Ē.ľ);if(ē.Length>4)float.TryParse(ē[4],out Ē.Ŀ);if(ē.Length>5)float.TryParse(ē[5],out Ē.ŀ);if(ē.Length>6)float.TryParse
(ē[6],out Ē.Ł);if(ē.Length>7)float.TryParse(ē[7],out Ē.ł);if(ē.Length>8)float.TryParse(ē[8],out Ē.Ń);int ń;if(ē.Length>9
&&int.TryParse(ē[9],out ń))Ē.Ņ=ń;if(ē.Length>10&&int.TryParse(ē[10],out ń))Ē.ņ=ń;if(ē.Length>11)Ē.Ň=ň(ē[11]);return Ē;}
private static r ď(string ù){r Ē=new r();if(string.IsNullOrEmpty(ù))return Ē;string[]ē=ù.Split(';');if(ē.Length==0||ē[0]!=Ĕ)
return new r();if(ē.Length>1)float.TryParse(ē[1],out Ē.ŉ);if(ē.Length>2)float.TryParse(ē[2],out Ē.Ŋ);if(ē.Length>3)float.
TryParse(ē[3],out Ē.ŋ);if(ē.Length>4)float.TryParse(ē[4],out Ē.Ō);if(ē.Length>5)float.TryParse(ē[5],out Ē.ō);if(ē.Length>6)float
.TryParse(ē[6],out Ē.Ŏ);if(ē.Length>7)float.TryParse(ē[7],out Ē.ŏ);if(ē.Length>8)float.TryParse(ē[8],out Ē.Ő);if(ē.Length
>9)float.TryParse(ē[9],out Ē.ő);if(ē.Length>10)float.TryParse(ē[10],out Ē.Œ);int œ;if(ē.Length>11&&int.TryParse(ē[11],out
œ))Ē.Ŕ=œ;if(ē.Length>12&&int.TryParse(ē[12],out œ))Ē.ŕ=œ;if(ē.Length>13&&int.TryParse(ē[13],out œ))Ē.Ŗ=œ;if(ē.Length>14)Ē
.ŗ=ň(ē[14]);return Ē;}private static l Đ(string ù){l Ē=new l();if(string.IsNullOrEmpty(ù))return Ē;string[]ē=ù.Split(';')
;if(ē.Length==0||ē[0]!=Ĕ)return new l();if(ē.Length>1)Ē.Ř=ı(ē[1]);if(ē.Length>2)Ē.ř=Ĵ(ē[2]);if(ē.Length>3)Ē.Ś=ı(ē[3]);
return Ē;}private static t đ(string ù){t Ē=new t();if(string.IsNullOrEmpty(ù))return Ē;string[]ē=ù.Split(';');if(ē.Length==0||
ē[0]!=Ĕ)return new t();if(ē.Length>1)Ē.Ň=ň(ē[1]);if(ē.Length>2)Ē.ŗ=ň(ē[2]);if(ē.Length>3)Ē.ś=ň(ē[3]);if(ē.Length>4)Ē.Ŝ=ň(
ē[4]);if(ē.Length>5)Ē.ŝ=ň(ē[5]);if(ē.Length>6)Ē.Ş=ň(ē[6]);if(ē.Length>7){int ş;if(int.TryParse(ē[7],out ş))Ē.Š=ş;}if(ē.
Length>8)Ē.š=ē[8];if(ē.Length>9)Ē.Ţ=ň(ē[9]);return Ē;}private static bool ň(string ţ){if(string.IsNullOrEmpty(ţ))return false;
if(ţ[0]=='1'&&ţ.Length==1)return true;if(ţ.Length==4&&(ţ[0]=='t'||ţ[0]=='T')&&(ţ[1]=='r'||ţ[1]=='R')&&(ţ[2]=='u'||ţ[2]==
'U')&&(ţ[3]=='e'||ţ[3]=='E'))return true;return false;}private static string[]ı(string ţ){if(ţ==null||ţ.Length==0)return
new string[0];int ť=Ť(ţ);string[]Ŧ=new string[ť];ŧ(ţ,Ŧ);return Ŧ;}private static float[]Ĵ(string ţ){if(ţ==null||ţ.Length==0
)return new float[0];int ť=Ũ(ţ);float[]ũ=new float[ť];int Ū=0;int ū=0;for(int ã=0;ã<=ţ.Length;ã++){if(ã==ţ.Length||ţ[ã]==
'|'){int â=ã-ū;string Ŭ=â>0?ţ.Substring(ū,â):string.Empty;float.TryParse(Ŭ,out ũ[Ū]);Ū++;ū=ã+1;}}return ũ;}private static
bool[]ĸ(string ţ){if(ţ==null||ţ.Length==0)return new bool[0];int ť=Ũ(ţ);bool[]ũ=new bool[ť];int Ū=0;int ū=0;for(int ã=0;ã<=ţ
.Length;ã++){if(ã==ţ.Length||ţ[ã]=='|'){int â=ã-ū;string Ŭ=â>0?ţ.Substring(ū,â):string.Empty;ũ[Ū]=ň(Ŭ);Ū++;ū=ã+1;}}return
ũ;}private static int Ť(string ţ){int ŭ=1;for(int ã=0;ã<ţ.Length;ã++){if(ţ[ã]=='\\'&&ã+1<ţ.Length){ã++;continue;}if(ţ[ã]
=='|')ŭ++;}return ŭ;}private static void ŧ(string ţ,string[]Ů){StringBuilder ů=new StringBuilder(32);int Ű=0;int ã=0;while
(ã<ţ.Length){char ű=ţ[ã];if(ű=='\\'&&ã+1<ţ.Length){char ť=ţ[ã+1];if(ť=='\\'||ť=='|')ů.Append(ť);else{ů.Append('\\');ů.
Append(ť);}ã+=2;}else if(ű=='|'){Ů[Ű++]=ů.ToString();ů.Length=0;ã++;}else{ů.Append(ű);ã++;}}Ů[Ű++]=ů.ToString();}private
static int Ũ(string ţ){int ŭ=1;for(int ã=0;ã<ţ.Length;ã++){if(ţ[ã]=='|')ŭ++;}return ŭ;}private const string Ĕ="1";}public
class A{sealed class Ŵ:º{private readonly A ų;public Ŵ(A f){ų=f;}public float v(A f,VRageMath.Vector2 g,VRageMath.RectangleF
h,string i,j k,l m,n o,p q,r s,t u){return ŵ(g,h,i,k,m);}public void µ(A f,MySpriteDrawFrame w,VRageMath.Vector2 g,
VRageMath.RectangleF x,string i,float y,float z,float ª,j k,l m,n o,p q,r s,t u){Ŷ(w,g,x,i,y,z,ª,k,m);}void ž(string ŷ,l Ÿ){Ź(ŷ,Ÿ
,ų.ź,ų.Ż,ų.ż,ų.Ž);}private const float ſ=0.55f;private static bool ƀ(VRageMath.Vector2 g,VRageMath.RectangleF h){if(g.X<
24f)return false;return h.Width<g.X*0.72f;}float ŵ(VRageMath.Vector2 g,VRageMath.RectangleF h,string ŷ,j Ɓ,l Ÿ){if(Ɓ==null
||Ÿ==null)return g.Y*0.12f;float Ƃ=g.Y*0.03515625f;bool ƃ=ƀ(g,h);float ƅ=ƃ?Ƅ(ſ,h.Width):ſ;float Ƈ=ƃ?Ɔ(g.Y,ƅ,ſ):Ƃ;int Ɖ=ƈ(h
.Width,ƅ);if(Ɖ<8)Ɖ=8;int Ƌ=Ɗ(Ɖ);float ƌ=g.Y*0.035f+g.Y*0.11f+g.Y*0.02f;float ƍ=g.Y*0.02f;bool Ǝ=!string.IsNullOrEmpty(ŷ);
float Ə=Ǝ?0f:(ƌ+g.Y*0.015f);ž(ŷ,Ÿ);int Ɛ=ų.ż.Count;int Ƒ=ų.Ž.Count;if(ƃ){int ƒ=0;for(int Ɠ=0;Ɠ<Ƒ;Ɠ++){string Ɣ=ų.Ž[Ɠ];float ƕ
=0f;float Ɩ=0f;ų.ź.TryGetValue(Ɣ,out ƕ);ų.Ż.TryGetValue(Ɣ,out Ɩ);string Ɨ;if(string.Equals(Ɣ,"Ice",Ƙ.ƙ)){float ƚ=ƕ+Ɩ;Ɨ=ƛ.
Ɯ(ƚ)+" "+Ɣ;}else{string Ɲ=ƛ.Ɯ(ƕ);string ƞ=ƛ.Ɯ(Ɩ);Ɨ=Ɲ+"/"+ƞ+" "+Ɣ;}ƒ+=Ɵ(Ɨ,Ɖ,Ƌ);}int Ơ=0;for(int ű=0;ű<Ɛ;ű++){int ơ=ų.ż[ű];
string Ƣ=Ÿ.Ř[ơ]??"";string ƣ=ƛ.Ɯ(Ÿ.ř[ơ]);string Ƥ=ƣ.PadLeft(6)+" "+Ƣ;Ơ+=Ɵ(Ƥ,Ɖ,Ƌ);}bool ƥ=Ƒ>0||Ɛ>0;if(!ƥ)return Ə+Ƈ+ƍ;int Ʀ;if(
Ǝ){if(Ƒ>0&&Ɛ>0)Ʀ=2;else Ʀ=1;}else Ʀ=2;return Ə+Ʀ*Ƈ+(ƒ+Ơ)*Ƈ+ƍ;}float Ƨ=h.Width;float ƨ=h.X;float Ʃ=ƨ+Ƨ*0.01953125f;float ƪ
=ƨ+Ƨ*0.52f;float ƫ=Math.Max(24f,ƪ-Ʃ-2f);float Ƭ=Math.Max(24f,(ƨ+Ƨ)-ƪ-2f);float ƭ=Math.Max(40f,Ƨ-Ƨ*0.04f);int Ʈ=Ǝ&&Ɛ==0?ƈ(
ƭ,ſ):ƈ(ƫ,ſ);int Ư=Ǝ&&Ƒ==0?ƈ(ƭ,ſ):ƈ(Ƭ,ſ);if(Ʈ<8)Ʈ=8;if(Ư<8)Ư=8;int ư=Ɗ(Ʈ);int Ʊ=Ɗ(Ư);int Ʋ=Math.Max(Ƒ,Ɛ);if(Ʋ==0)return Ə+
Ƃ+ƍ;int Ƴ=Ǝ?((Ƒ>0||Ɛ>0)?1:0):1;int ƴ=0;for(int í=0;í<Ʋ;í++){int Ƶ=0;int ƶ=0;if(í<Ƒ){string Ɣ=ų.Ž[í];float ƕ=0f;float Ɩ=0f
;ų.ź.TryGetValue(Ɣ,out ƕ);ų.Ż.TryGetValue(Ɣ,out Ɩ);string Ɨ;if(string.Equals(Ɣ,"Ice",Ƙ.ƙ)){float ƚ=ƕ+Ɩ;Ɨ=ƛ.Ɯ(ƚ)+" "+Ɣ;}
else{string Ɲ=ƛ.Ɯ(ƕ);string ƞ=ƛ.Ɯ(Ɩ);Ɨ=Ɲ+"/"+ƞ+" "+Ɣ;}Ƶ=Ɵ(Ɨ,Ʈ,ư);}if(í<Ɛ){int ơ=ų.ż[í];string Ƣ=Ÿ.Ř[ơ]??"";string ƣ=ƛ.Ɯ(Ÿ.ř[
ơ]);string Ƥ=ƣ.PadLeft(6)+" "+Ƣ;ƶ=Ɵ(Ƥ,Ư,Ʊ);}int Ʒ=Math.Max(1,Math.Max(Ƶ,ƶ));ƴ+=Ʒ;}return Ə+Ƴ*Ƃ+ƴ*Ƃ+ƍ;}void Ŷ(
MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,string ŷ,float y,float Ƹ,float ƹ,j Ɓ,l Ÿ){if(Ɓ==null||Ÿ==null)return;if(Ÿ.
Ř==null||Ÿ.ř==null||Ÿ.Ś==null)return;float Ƨ=x.Width;float ƨ=x.X;float ƺ=ŵ(g,x,ŷ,Ɓ,Ÿ);if(y+ƺ<=Ƹ||y>=ƹ)return;bool ƃ=ƀ(g,x
);float Ƃ=g.Y*0.03515625f;float Ƈ=Ƃ;float ƻ=ſ;if(ƃ){ƻ=Ƅ(ſ,Ƨ);Ƈ=Ɔ(g.Y,ƻ,ſ);}bool Ǝ=!string.IsNullOrEmpty(ŷ);ž(ŷ,Ÿ);int Ƒ=ų
.Ž.Count;int Ƽ=ų.ż.Count;float ƽ=ƹ+(g.Y*0.01f);if(ƃ){float ƾ=Math.Max(2f,Ƨ*0.02f);float ƿ=ƨ+ƾ;int ǀ=ƈ(Ƨ,ƻ);if(ǀ<8)ǀ=8;int
ǁ=Ɗ(ǀ);float ǂ=y;if(!Ǝ){float ǅ=Ɓ.Į>0.0001f?ǃ.Ǆ(Ɓ.ĭ/Ɓ.Į,0f,1f):0f;string Ǉ=ƛ.ǆ(Ɓ.į);string Ʒ=ƛ.Ɯ(Ɓ.ĭ);string ǈ=ƛ.Ɯ(Ɓ.Į);
var ǉ=new[]{"Cargo"};var Ǌ=new[]{ǅ};var ǋ=new[]{Ʒ+" / "+ǈ+" L "+Ǉ};var ǌ=new VRageMath.Color(0,0,255,200);float Ǎ=ƨ+Ƨ*0.5f;
float Ǐ=ų.ǎ(y,new VRageMath.Vector2(Ƨ,g.Y),new VRageMath.Vector2(Ǎ,0f),ǉ,Ǌ,ǋ,ǌ,true);ǂ=y+Ǐ+g.Y*0.015f;ų.ǐ("ORES/INGOTS",ƿ,ǂ,ƻ
,A.Ǒ,A.ǒ,TextAlignment.LEFT);ǂ+=Ƈ;for(int Ɠ=0;Ɠ<Ƒ;Ɠ++){string Ɣ=ų.Ž[Ɠ];float ƕ=0f;float Ɩ=0f;ų.ź.TryGetValue(Ɣ,out ƕ);ų.Ż
.TryGetValue(Ɣ,out Ɩ);string Ɨ;if(string.Equals(Ɣ,"Ice",Ƙ.ƙ)){float ƚ=ƕ+Ɩ;Ɨ=ƛ.Ɯ(ƚ)+" "+Ɣ;}else{string Ɲ=ƛ.Ɯ(ƕ);string ƞ=ƛ
.Ɯ(Ɩ);Ɨ=Ɲ+"/"+ƞ+" "+Ɣ;}if(ǂ+Ƈ>Ƹ&&ǂ<ƽ){int ǖ=ų.Ǔ(Ɨ,ƿ,ǂ,Ƈ,ƻ,A.ǔ,A.Ǖ,TextAlignment.LEFT,ǀ,ǁ,true);ǂ+=ǖ*Ƈ;}else{int ǖ=Ɵ(Ɨ,ǀ,ǁ
);ǂ+=ǖ*Ƈ;}}ų.ǐ("COMPONENTS",ƿ,ǂ,ƻ,A.Ǒ,A.ǒ,TextAlignment.LEFT);ǂ+=Ƈ;for(int ű=0;ű<Ƽ;ű++){int ơ=ų.ż[ű];string Ƣ=Ÿ.Ř[ơ]??"";
string ƣ=ƛ.Ɯ(Ÿ.ř[ơ]);string Ƥ=ƣ.PadLeft(6)+" "+Ƣ;if(ǂ+Ƈ>Ƹ&&ǂ<ƽ){int Ǘ=ų.Ǔ(Ƥ,ƿ,ǂ,Ƈ,ƻ,A.ǔ,A.Ǖ,TextAlignment.LEFT,ǀ,ǁ,true);ǂ+=Ǘ*
Ƈ;}else{int Ǘ=Ɵ(Ƥ,ǀ,ǁ);ǂ+=Ǘ*Ƈ;}}}else{float ǘ=y;if(Ƒ>0&&Ƽ>0){ų.ǐ("ORES/INGOTS",ƿ,ǘ,ƻ,A.Ǒ,A.ǒ,TextAlignment.LEFT);ų.ǐ(
"COMPONENTS",ƿ,ǘ+Ƈ,ƻ,A.Ǒ,A.ǒ,TextAlignment.LEFT);ǂ=ǘ+Ƈ*2f;}else if(Ƒ>0){ų.ǐ("ORES/INGOTS",ƿ,ǘ,ƻ,A.Ǒ,A.ǒ,TextAlignment.LEFT);ǂ=ǘ+Ƈ;}
else if(Ƽ>0){ų.ǐ("COMPONENTS",ƿ,ǘ,ƻ,A.Ǒ,A.ǒ,TextAlignment.LEFT);ǂ=ǘ+Ƈ;}else ǂ=y;for(int Ɠ=0;Ɠ<Ƒ;Ɠ++){string Ɣ=ų.Ž[Ɠ];float ƕ
=0f;float Ɩ=0f;ų.ź.TryGetValue(Ɣ,out ƕ);ų.Ż.TryGetValue(Ɣ,out Ɩ);string Ɨ;if(string.Equals(Ɣ,"Ice",Ƙ.ƙ)){float ƚ=ƕ+Ɩ;Ɨ=ƛ.
Ɯ(ƚ)+" "+Ɣ;}else{string Ɲ=ƛ.Ɯ(ƕ);string ƞ=ƛ.Ɯ(Ɩ);Ɨ=Ɲ+"/"+ƞ+" "+Ɣ;}if(ǂ+Ƈ>Ƹ&&ǂ<ƽ){int Ʒ=ų.Ǔ(Ɨ,ƿ,ǂ,Ƈ,ƻ,A.ǔ,A.Ǖ,
TextAlignment.LEFT,ǀ,ǁ,true);ǂ+=Ʒ*Ƈ;}else{int Ʒ=Ɵ(Ɨ,ǀ,ǁ);ǂ+=Ʒ*Ƈ;}}for(int ű=0;ű<Ƽ;ű++){int ơ=ų.ż[ű];string Ƣ=Ÿ.Ř[ơ]??"";string ƣ=ƛ.Ɯ(
Ÿ.ř[ơ]);string Ƥ=ƣ.PadLeft(6)+" "+Ƣ;if(ǂ+Ƈ>Ƹ&&ǂ<ƽ){int Ʒ=ų.Ǔ(Ƥ,ƿ,ǂ,Ƈ,ƻ,A.ǔ,A.Ǖ,TextAlignment.LEFT,ǀ,ǁ,true);ǂ+=Ʒ*Ƈ;}else{
int Ʒ=Ɵ(Ƥ,ǀ,ǁ);ǂ+=Ʒ*Ƈ;}}}return;}float Ǚ;if(!Ǝ){float ǅ=Ɓ.Į>0.0001f?ǃ.Ǆ(Ɓ.ĭ/Ɓ.Į,0f,1f):0f;string Ǉ=ƛ.ǆ(Ɓ.į);string Ʒ=ƛ.Ɯ(Ɓ.
ĭ);string ǈ=ƛ.Ɯ(Ɓ.Į);var ǉ=new[]{"Cargo"};var Ǌ=new[]{ǅ};var ǋ=new[]{Ʒ+" / "+ǈ+" L "+Ǉ};var ǌ=new VRageMath.Color(0,0,255
,200);float Ǎ=ƨ+Ƨ*0.5f;float Ǐ=ų.ǎ(y,new VRageMath.Vector2(Ƨ,g.Y),new VRageMath.Vector2(Ǎ,0f),ǉ,Ǌ,ǋ,ǌ,true);float ǚ=y+Ǐ+g
.Y*0.015f;float Ǜ=ƨ+Ƨ*0.01953125f;float ǜ=ƨ+Ƨ*0.52f;ų.ǐ("ORES/INGOTS",Ǜ,ǚ,ſ,A.Ǒ,A.ǒ,TextAlignment.LEFT);ų.ǐ("COMPONENTS",
ǜ,ǚ,ſ,A.Ǒ,A.ǒ,TextAlignment.LEFT);Ǚ=ǚ+Ƃ;}else{float ǘ=y;if(Ƒ>0&&Ƽ>0){ų.ǐ("ORES/INGOTS",ƨ+Ƨ*0.01953125f,ǘ,ſ,A.Ǒ,A.ǒ,
TextAlignment.LEFT);ų.ǐ("COMPONENTS",ƨ+Ƨ*0.52f,ǘ,ſ,A.Ǒ,A.ǒ,TextAlignment.LEFT);Ǚ=ǘ+Ƃ;}else if(Ƒ>0){ų.ǐ("ORES/INGOTS",ƨ+Ƨ*0.01953125f,
ǘ,ſ,A.Ǒ,A.ǒ,TextAlignment.LEFT);Ǚ=ǘ+Ƃ;}else if(Ƽ>0){ų.ǐ("COMPONENTS",ƨ+Ƨ*0.01953125f,ǘ,ſ,A.Ǒ,A.ǒ,TextAlignment.LEFT);Ǚ=ǘ+
Ƃ;}else Ǚ=y;}float Ʃ=ƨ+Ƨ*0.01953125f;float ƪ=ƨ+Ƨ*0.52f;float ƫ=Math.Max(24f,ƪ-Ʃ-2f);float Ƭ=Math.Max(24f,(ƨ+Ƨ)-ƪ-2f);
float ƭ=Math.Max(40f,Ƨ-Ƨ*0.04f);int Ʈ=Ǝ&&Ƽ==0?ƈ(ƭ,ſ):ƈ(ƫ,ſ);int Ư=Ǝ&&Ƒ==0?ƈ(ƭ,ſ):ƈ(Ƭ,ſ);if(Ʈ<8)Ʈ=8;if(Ư<8)Ư=8;int ư=Ɗ(Ʈ);int
Ʊ=Ɗ(Ư);float ǝ=Ǚ;int Ʋ=Math.Max(Ƒ,Ƽ);float Ǟ=ǝ;for(int í=0;í<Ʋ;í++){string ǟ=null;string Ǡ=null;int Ƶ=0;int ƶ=0;if(í<Ƒ){
string Ɣ=ų.Ž[í];float ƕ=0f;float Ɩ=0f;ų.ź.TryGetValue(Ɣ,out ƕ);ų.Ż.TryGetValue(Ɣ,out Ɩ);if(string.Equals(Ɣ,"Ice",Ƙ.ƙ)){float ƚ
=ƕ+Ɩ;ǟ=ƛ.Ɯ(ƚ)+" "+Ɣ;}else{string Ɲ=ƛ.Ɯ(ƕ);string ƞ=ƛ.Ɯ(Ɩ);ǟ=Ɲ+"/"+ƞ+" "+Ɣ;}Ƶ=Ɵ(ǟ,Ʈ,ư);}if(í<Ƽ){int ơ=ų.ż[í];string Ƣ=Ÿ.Ř[
ơ]??"";string ƣ=ƛ.Ɯ(Ÿ.ř[ơ]);Ǡ=ƣ.PadLeft(6)+" "+Ƣ;ƶ=Ɵ(Ǡ,Ư,Ʊ);}int ǡ=Math.Max(1,Math.Max(Ƶ,ƶ));if(Ǟ+Ƃ>Ƹ&&Ǟ<ƽ){if(ǟ!=null){
float Ǣ=Ǝ&&Ƽ==0?ƨ+Ƨ*0.01953125f:Ʃ;ų.Ǔ(ǟ,Ǣ,Ǟ,Ƃ,ſ,A.ǔ,A.Ǖ,TextAlignment.LEFT,Ʈ,ư,true);}if(Ǡ!=null){float ǣ=Ǝ&&Ƒ==0?ƨ+Ƨ*
0.01953125f:ƪ;ų.Ǔ(Ǡ,ǣ,Ǟ,Ƃ,ſ,A.ǔ,A.Ǖ,TextAlignment.LEFT,Ư,Ʊ,true);}}Ǟ+=ǡ*Ƃ;}}}private const string Ǥ="[GbearOS]",ǥ="[Manual]",ǒ=
"White",Ǖ="Monospace",Ǧ="SquareSimple";private static readonly VRageMath.Color ǔ=VRageMath.Color.White,ǧ=new VRageMath.Color(
255,0,0,255),Ǩ=new VRageMath.Color(0,255,0,255),Ǒ=new VRageMath.Color(128,128,128,255),ǩ=new VRageMath.Color(0,0,0,255),Ǫ=
new VRageMath.Color(38,42,48,255);enum Ǵ{ǫ,Ǭ,ǭ,Ǯ,ǯ,ǰ,Ǳ,ǲ,ǳ,}struct Ǹ{public Ǵ ǵ;public string Ƕ,Ƿ;}struct ȁ{public
IMyTextPanel ǹ;public List<Ǹ>Ǻ;public float ǻ,Ǽ,ǽ,Ǿ;public int ǿ;public bool Ȁ;}IMyGridTerminalSystem Ȃ;IMyProgrammableBlock À;C D;
private readonly List<IMyTextPanel>ȃ=new List<IMyTextPanel>(64);private readonly List<ȁ>Ȅ=new List<ȁ>(64),ȅ=new List<ȁ>(64);
private readonly List<MySprite>Ȇ=new List<MySprite>(320);private readonly ȇ Ȉ=new ȇ(),ȉ=new ȇ();private readonly List<string>Ȋ=
new List<string>(8),Ž=new List<string>(128),ȋ=new List<string>(16),Ȍ=new List<string>(16);private readonly Dictionary<
string,float>ź=new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase),Ż=new Dictionary<string,float>(StringComparer.
OrdinalIgnoreCase);private readonly List<int>ż=new List<int>(128);E F;bool ȍ,Ȏ,ȏ,Ȑ,ȑ,Ȓ;int ȓ=0;float Ȕ=-1f;float ȕ=9999f;j Ȗ;n ȗ;p Ș;r ș;
l Ț;t ț;bool Ȝ;Dictionary<string,º>ȝ;private static void Ȟ(IMyTextPanel ē){if(ē==null)return;ē.ContentType=ContentType.
SCRIPT;ē.Script="";ē.ScriptBackgroundColor=VRageMath.Color.Black;}private static void ȡ(MySpriteDrawFrame w,VRageMath.Vector2
ȟ,VRageMath.Vector2 Ƞ){w.Add(new MySprite{Type=SpriteType.TEXTURE,Data=Ǧ,Position=Ƞ,Size=ȟ,Color=ǩ,Alignment=
TextAlignment.CENTER,RotationOrScale=0f,});}void ǐ(string Ȣ,float ȣ,float Ȥ,float ţ,VRageMath.Color ű,string ȥ,TextAlignment Ȧ){if(Ȕ
>=0f&&(Ȥ<Ȕ||Ȥ>ȕ))return;Ȇ.Add(new MySprite{Type=SpriteType.TEXT,Data=Ȣ,Position=new VRageMath.Vector2(ȣ,Ȥ),Color=ű,FontId=
ȥ,Alignment=Ȧ,RotationOrScale=ţ,});}void ȩ(string Ē,float ȣ,float Ȥ,float ȧ,float Ȩ,VRageMath.Color ű){if(Ȕ>=0f&&(Ȥ-(Ȩ*
0.5f)<Ȕ||Ȥ+(Ȩ*0.5f)>ȕ))return;Ȇ.Add(new MySprite{Type=SpriteType.TEXTURE,Data=Ē,Position=new VRageMath.Vector2(ȣ,Ȥ),Size=new
VRageMath.Vector2(ȧ,Ȩ),Color=ű,Alignment=TextAlignment.CENTER,RotationOrScale=0f,});}void ȱ(float Ȫ,float Ȥ,float ȫ,float Ȭ,float
ȭ,float ǅ,VRageMath.Color Ȯ){ȩ(Ǧ,Ȫ,Ȥ,ȫ,Ȭ,Ǫ);float í=ǃ.Ǆ(ǅ,0f,1f);if(í<=1e-5f)return;float ȯ=ǃ.Ǆ(Math.Max(ȭ,í*ȫ),ȭ,ȫ);
float Ȱ=Ȫ-ȫ*0.5f+ȯ*0.5f;ȩ(Ǧ,Ȱ,Ȥ,ȯ,Ȭ,Ȯ);}float ǎ(float y,VRageMath.Vector2 ȟ,VRageMath.Vector2 Ƞ,string[]Ȳ,float[]ȳ,string[]ȴ,
VRageMath.Color ȵ,bool ȶ){float ȷ=ȟ.Y*0.11f;float Ȭ=ȟ.Y*0.045f;float ȸ=ȟ.X*0.02f;float ȫ=ȟ.X-2f*ȸ;float ȹ=Ȭ*0.35f;int ť=Ȳ.Length;
float Ⱥ=y+ȟ.Y*0.035f;for(int ã=0;ã<ť;ã++){float Ȥ=Ⱥ+ã*ȷ;if(ȶ)ȱ(Ƞ.X,Ȥ,ȫ,Ȭ,ȹ,ȳ[ã],ȵ);if(ȶ)ǐ(Ȳ[ã]+" "+ȴ[ã],Ƞ.X,Ȥ+Ȭ*0.55f,0.55f,
new VRageMath.Color(230,230,230,255),ǒ,TextAlignment.CENTER);}return ȟ.Y*0.035f+ť*ȷ+ȟ.Y*0.02f;}public void H(
IMyGridTerminalSystem Ȼ,IMyProgrammableBlock Ñ,C ȼ,E Ƚ){Ȃ=Ȼ;À=Ñ;D=ȼ;F=Ƚ;ȓ=0;ȝ=new Dictionary<string,º>(StringComparer.OrdinalIgnoreCase);ȝ[
"INV"]=new Ŵ(this);ȝ["PWR"]=new Ⱦ(this);ȝ["ICE"]=new ȿ(this);ȝ["REF"]=new ɀ(this);ȝ["WARN"]=new Ɂ(this);ȝ["STATUS"]=new ɂ(
this);}º Ʌ(string Ƀ){if(ȝ==null||Ƀ==null)return null;º Ʉ;return ȝ.TryGetValue(Ƀ,out Ʉ)?Ʉ:null;}private static Ǵ Ɉ(string Ɇ){
if(string.IsNullOrEmpty(Ɇ))return Ǵ.ǫ;if(string.Equals(Ɇ,"HEAD",Ƙ.ƙ))return Ǵ.Ǭ;if(string.Equals(Ɇ,"INV",Ƙ.ƙ))return Ǵ.ǭ;
if(string.Equals(Ɇ,"REF",Ƙ.ƙ))return Ǵ.Ǯ;if(string.Equals(Ɇ,"PWR",Ƙ.ƙ))return Ǵ.ǯ;if(string.Equals(Ɇ,"ICE",Ƙ.ƙ))return Ǵ.ǰ
;if(string.Equals(Ɇ,"WARN",Ƙ.ƙ))return Ǵ.Ǳ;if(string.Equals(Ɇ,"STATUS",Ƙ.ƙ))return Ǵ.ǲ;if(string.Equals(Ɇ,ȇ.ɇ,Ƙ.ƙ))return
Ǵ.ǳ;return Ǵ.ǫ;}private static string Ɋ(ref Ǹ ɉ){if(ɉ.ǵ==Ǵ.ǫ)return ɉ.Ƿ;switch(ɉ.ǵ){case Ǵ.ǭ:return"INV";case Ǵ.Ǯ:return
"REF";case Ǵ.ǯ:return"PWR";case Ǵ.ǰ:return"ICE";case Ǵ.Ǳ:return"WARN";case Ǵ.ǲ:return"STATUS";default:return null;}}private
static string Ɍ(Ǵ Ȣ,string ɋ){switch(Ȣ){case Ǵ.ǭ:return"INVENTORY";case Ǵ.Ǯ:return"REFINERY STATUS";case Ǵ.ǰ:return
"ICE STATUS";case Ǵ.ǯ:return"POWER GRID STATUS";case Ǵ.Ǳ:return"WARNING STATUS";case Ǵ.ǲ:return"SYSTEM STATUS";case Ǵ.ǫ:return ɋ!=
null?ɋ:"";default:return"";}}public void W(double ɍ){if(Ȃ==null||À==null)return;Ɏ();string ɏ="Offline for: "+ɍ.ToString("F0"
)+"s";int ŭ=Ȅ.Count;for(int ã=0;ã<ŭ;ã++){IMyTextPanel ɐ=Ȅ[ã].ǹ;if(ɐ==null)continue;Ȟ(ɐ);VRageMath.Vector2 ȟ;VRageMath.
Vector2 Ƞ;ɑ(ɐ,out ȟ,out Ƞ);using(var w=ɐ.DrawFrame()){ȡ(w,ȟ,Ƞ);Ȇ.Clear();ǐ("NO SIGNAL",Ƞ.X,ȟ.Y*0.10f,1.35f,ǧ,ǒ,TextAlignment.
CENTER);ǐ("WAITING FOR TELEMETRY...",Ƞ.X,ȟ.Y*0.20f,0.72f,ǔ,ǒ,TextAlignment.CENTER);ǐ(ɏ,Ƞ.X,ȟ.Y*0.28f,0.62f,Ǒ,ǒ,TextAlignment.
CENTER);ɒ(w);}}}public void X(j k,n o,p q,r s,l m,t u,bool O){if(Ȃ==null||À==null)return;Ɏ();if(O){Ȝ=ɓ(Ȗ,k);Ȏ=ɓ(ȗ,o);ȏ=ɓ(Ș,q);
Ȑ=ɓ(ș,s);ȑ=ɓ(Ț,m);Ȓ=ɓ(ț,u);ȍ=ɔ();Ȗ=k;ȗ=o;Ș=q;ș=s;Ț=m;ț=u;}ɕ(k,o,q,s,m,u);ɖ(k,o,q,s,m,u);}private static bool ɓ<ċ>(ċ Ȧ,ċ ɗ
){if(Ȧ==null&&ɗ==null)return false;if(Ȧ==null||ɗ==null)return true;return!Ȧ.Equals(ɗ);}bool ɛ(List<Ǹ>ɘ){if(ɘ==null||ɘ.
Count==0)return false;bool ə=Ȝ||ȑ||Ȏ||ȏ||Ȑ||Ȓ||ȍ;bool ɚ=Ȝ||ȑ;int ť=ɘ.Count;for(int ã=0;ã<ť;ã++){switch(ɘ[ã].ǵ){case Ǵ.ǭ:if(ɚ)
return true;break;case Ǵ.Ǯ:if(Ȏ)return true;break;case Ǵ.ǰ:if(ȏ)return true;break;case Ǵ.ǯ:if(Ȑ)return true;break;case Ǵ.Ǳ:if(
Ȓ)return true;break;case Ǵ.ǲ:if(ȍ)return true;break;case Ǵ.ǫ:if(ə)return true;break;}}return false;}void ɒ(
MySpriteDrawFrame w){int ť=Ȇ.Count;for(int ɜ=0;ɜ<ť;ɜ++)w.Add(Ȇ[ɜ]);Ȇ.Clear();}void ɖ(j k,n o,p q,r s,l m,t u){int ŭ=Ȅ.Count;for(int ã=0;ã
<ŭ;ã++){var M=Ȅ[ã];if(M.Ǻ==null||M.Ǻ.Count==0)continue;if(!ɛ(M.Ǻ)&&!M.Ȁ)continue;ɝ(ref M,k,o,q,s,m,u);M.Ȁ=false;Ȅ[ã]=M;}}
void ɕ(j k,n o,p q,r s,l m,t u){int ŭ=Ȅ.Count;for(int ã=0;ã<ŭ;ã++){var M=Ȅ[ã];if(M.Ǻ==null)continue;VRageMath.Vector2 ɞ,ɟ;ɑ(
M.ǹ,out ɞ,out ɟ);float ɠ=ɞ.Y*0.95703125f;float ɡ,ɢ;ɣ(Ȉ,M.Ǻ,ɞ,k,o,q,s,m,u,out ɡ,out ɢ);M.ǽ=ɡ;M.Ǿ=ɢ;float ɤ=ɠ-ɡ;if(ɢ>ɤ){
float ɥ=ɢ-ɤ;float ɦ=ɤ*0.90f;if(M.Ǽ>M.ǻ){float ɧ=ɦ/12f;M.ǻ+=ɧ;if(M.ǻ>=M.Ǽ)M.ǻ=M.Ǽ;M.Ȁ=true;}else if(M.Ǽ<M.ǻ){float ɨ=M.ǻ-M.Ǽ;
float ɩ=ɨ*0.15f;if(ɩ<20f)ɩ=20f;M.ǻ-=ɩ;if(M.ǻ<=M.Ǽ)M.ǻ=M.Ǽ;M.Ȁ=true;}else{M.ǿ++;if(M.ǿ>=30){M.ǿ=0;if(M.ǻ>=ɥ-5f){M.Ǽ=0f;}else{M
.Ǽ=M.ǻ+ɦ;if(M.Ǽ>ɥ)M.Ǽ=ɥ;}M.Ȁ=true;}}}else{M.ǻ=0f;M.Ǽ=0f;M.ǿ=0;}Ȅ[ã]=M;}}void Ɏ(){if(ȓ>0){ȓ--;return;}ȓ=100;ȃ.Clear();Ȃ.
GetBlocksOfType(ȃ,ɪ);ȅ.Clear();for(int ɫ=0;ɫ<Ȅ.Count;ɫ++)ȅ.Add(Ȅ[ɫ]);Ȅ.Clear();int ť=ȃ.Count;for(int ã=0;ã<ť;ã++){var ē=ȃ[ã];if(ē==null
)continue;string Ƣ=ē.CustomName;if(ɬ.ɭ(Ƣ,ǥ))continue;ȁ M;M.ǹ=ē;M.ǻ=0f;M.Ǽ=0f;M.ǿ=0;M.Ȁ=false;M.ǽ=0f;M.Ǿ=0f;for(int ɮ=0;ɮ<
ȅ.Count;ɮ++){if(ȅ[ɮ].ǹ==ē){M.ǻ=ȅ[ɮ].ǻ;M.Ǽ=ȅ[ɮ].Ǽ;M.ǿ=ȅ[ɮ].ǿ;break;}}if(!ɬ.ɭ(Ƣ,Ǥ))continue;var ɘ=new List<Ǹ>(8);ɯ(ē.
CustomData,ɘ);if(ɘ.Count==0)continue;M.Ǻ=ɘ;Ȅ.Add(M);}}void ɯ(string ɰ,List<Ǹ>ü){ü.Clear();bool ɱ=string.IsNullOrWhiteSpace(ɰ);if(ɱ
){ü.Add(new Ǹ{ǵ=Ǵ.ǭ,Ƕ="",Ƿ=null});return;}int ɲ=0;int â=ɰ.Length;while(ɲ<â){int ɳ=ɰ.IndexOf('\n',ɲ);string ɴ=ɳ<0?ɰ.
Substring(ɲ):ɰ.Substring(ɲ,ɳ-ɲ);ɲ=ɳ<0?â:ɳ+1;int ǉ=ɴ.IndexOf('[');int ɵ=ɴ.IndexOf(']');if(ǉ<0||ɵ<=ǉ)continue;string ɶ=ɴ.Substring(
ǉ+1,ɵ-ǉ-1).Trim();if(ɶ.Length==0)continue;Ǹ ɷ;int ű=ɶ.IndexOf(':');string ɸ;if(ű<0){ɸ=ɶ.Trim();ɷ.Ƕ="";}else{ɸ=ɶ.Substring
(0,ű).Trim();ɷ.Ƕ=ɶ.Substring(ű+1).Trim();}if(ɸ.Length==0)continue;ɷ.ǵ=Ɉ(ɸ);if(ɷ.ǵ==Ǵ.ǫ)ɷ.Ƿ=ɸ;else ɷ.Ƿ=null;ü.Add(ɷ);}}
bool ɪ(IMyTextPanel ē){if(ē==null)return false;if(!ē.IsSameConstructAs(À))return false;return true;}private static void ɑ(
IMyTextPanel ɐ,out VRageMath.Vector2 ȟ,out VRageMath.Vector2 Ƞ){var ɹ=ɐ as IMyTextSurface;var ɺ=ɹ!=null?ɹ.TextureSize:default(
VRageMath.Vector2);var ɻ=ɹ!=null?ɹ.SurfaceSize:default(VRageMath.Vector2);ȟ=(ɺ.X>=8f&&ɺ.Y>=8f)?ɺ:((ɻ.X>=8f&&ɻ.Y>=8f)?ɻ:new
VRageMath.Vector2(512f,512f));Ƞ=ȟ*0.5f;}float ɼ(VRageMath.Vector2 ȟ){return ȟ.Y*0.045f;}float ɿ(float ɽ,VRageMath.Vector2 ȟ,float
Ȫ,string ɾ,bool ȶ){float Ȩ=ɼ(ȟ);if(ȶ)ǐ("--- "+ɾ+" ---",Ȫ,ɽ,0.55f,Ǒ,ǒ,TextAlignment.CENTER);return Ȩ;}void ɣ(ȇ ʀ,List<Ǹ>ɘ,
VRageMath.Vector2 ȟ,j k,n o,p q,r s,l m,t u,out float Ƹ,out float ʁ){Ƹ=ȟ.Y*0.02f;ʀ.ʂ(ȟ.X,ȟ.Y);int ʃ=ɘ.Count;for(int ã=0;ã<ʃ;ã++){
var ű=ɘ[ã];switch(ű.ǵ){case Ǵ.Ǭ:Ƹ+=ȟ.Y*0.07f;continue;case Ǵ.ǳ:ʀ.ʄ(ű.Ƕ);continue;}bool ʅ=(ű.ǵ==Ǵ.ǭ||ű.ǵ==Ǵ.ǲ)&&!string.
IsNullOrEmpty(ű.Ƕ);float ʆ=ʅ?0f:ɼ(ȟ);float ʈ=ʇ(ű,ʀ,ȟ,k,o,q,s,m,u);ʀ.ʉ(ʆ+ʈ);}ʀ.ʊ();ʁ=ʀ.ʋ;}float ʇ(Ǹ ɉ,ȇ ʀ,VRageMath.Vector2 ȟ,j k,n o,
p q,r s,l m,t u){if(ɉ.ǵ==Ǵ.ǳ)return 0f;string ʌ=Ɋ(ref ɉ);º Ʉ=Ʌ(ʌ);if(Ʉ!=null)return Ʉ.v(this,ȟ,ʀ.ʍ,ɉ.Ƕ,k,m,o,q,s,u);
return ȟ.Y*0.04f;}void ɝ(ref ȁ ʎ,j k,n o,p q,r s,l m,t u){IMyTextPanel ɐ=ʎ.ǹ;if(ɐ==null)return;Ȟ(ɐ);VRageMath.Vector2 ȟ;
VRageMath.Vector2 Ƞ;ɑ(ɐ,out ȟ,out Ƞ);float ƹ=ȟ.Y*0.95703125f;float Ƹ=ʎ.ǽ;float ʏ=ʎ.Ǿ;float ʐ=ƹ-Ƹ;float ʑ=ȟ.Y*0.02f;float ʒ=Ƹ+ʑ-ʎ.
ǻ;using(var w=ɐ.DrawFrame()){ȡ(w,ȟ,Ƞ);Ȇ.Clear();float ʓ=ȟ.Y*0.025f;int ʃ=ʎ.Ǻ.Count;for(int ã=0;ã<ʃ;ã++){var ű=ʎ.Ǻ[ã];if(ű
.ǵ!=Ǵ.Ǭ)continue;string ʔ=string.IsNullOrEmpty(ű.Ƕ)?" ":ű.Ƕ;ǐ(ʔ,Ƞ.X,ʓ,0.88f,ǔ,ǒ,TextAlignment.CENTER);ʓ+=ȟ.Y*0.07f;}if(ʏ>
ʐ){float ʕ=ʏ-ʐ;float ʖ=ʐ*0.90f;int ʗ=(int)Math.Ceiling(ʕ/ʖ)+1;int ʘ;if(ʎ.ǻ>=ʕ-5f)ʘ=ʗ;else ʘ=(int)(ʎ.ǻ/ʖ)+1;ǐ("PAGE "+ʘ+
"/"+ʗ,ȟ.X*0.97f,ȟ.Y*0.025f,0.5f,new VRageMath.Color(180,180,180,255),ǒ,TextAlignment.RIGHT);}Ȕ=Ƹ+ʑ;ȕ=ƹ;ȉ.ʂ(ȟ.X,ȟ.Y);for(int
ã=0;ã<ʃ;ã++){var ű=ʎ.Ǻ[ã];switch(ű.ǵ){case Ǵ.Ǭ:continue;case Ǵ.ǳ:ȉ.ʄ(ű.Ƕ);continue;}bool ʅ=(ű.ǵ==Ǵ.ǭ||ű.ǵ==Ǵ.ǲ)&&!string.
IsNullOrEmpty(ű.Ƕ);float ʆ=ʅ?0f:ɼ(ȟ);float ʈ=ʇ(ű,ȉ,ȟ,k,o,q,s,m,u);float ʚ=ʒ+ȉ.ʙ;float ʛ=ʚ+ʆ+ʈ;bool ʜ=ʛ<=Ƹ||ʚ>=ƹ;if(!ʜ){if(!ʅ)ɿ(ʚ,ȟ,ȉ.
ʝ,Ɍ(ű.ǵ,ű.Ƿ),true);float Ⱥ=ʚ+ʆ;ʞ(ű,ȉ,w,k,o,q,s,m,u,ȟ,Ⱥ,Ƹ,ƹ);}ȉ.ʉ(ʆ+ʈ);}ȉ.ʊ();Ȕ=-1f;ɒ(w);}}void ʞ(Ǹ ɉ,ȇ ʀ,
MySpriteDrawFrame w,j k,n o,p q,r s,l m,t u,VRageMath.Vector2 ȟ,float y,float Ƹ,float ƹ){string ʌ=Ɋ(ref ɉ);º Ʉ=Ʌ(ʌ);if(Ʉ!=null){Ʉ.µ(this,
w,ȟ,ʀ.ʍ,ɉ.Ƕ,y,Ƹ,ƹ,k,m,o,q,s,u);}}bool ɔ(){if(F==null)return false;F.ā(ȋ);bool ʟ=ȋ.Count!=Ȍ.Count;if(!ʟ){for(int ã=0;ã<ȋ.
Count;ã++){string Ȧ=ȋ[ã]??"";string ɗ=ã<Ȍ.Count?(Ȍ[ã]??""):"";if(!string.Equals(Ȧ,ɗ,Ƙ.ƙ)){ʟ=true;break;}}}if(!ʟ)return false;
Ȍ.Clear();for(int ã=0;ã<ȋ.Count;ã++)Ȍ.Add(ȋ[ã]??"");return true;}private const float ʠ=0.45f;private const int ʡ=2;
private const string ʢ="  ";internal static int ƈ(float ʣ,float ʤ){float ʥ=ʣ*0.80f;if(ʥ<8f)ʥ=Math.Max(1f,ʣ*0.5f);float ʦ=19.5f*
ʤ;if(ʦ<=0.0001f)return 4;int ť=(int)(ʥ/ʦ);return ť<1?1:ť;}internal static float Ƅ(float ʧ,float ʨ){float ȧ=ʨ>2f?ʨ:400f;
float ʩ=520f;float ʪ=ʧ*Math.Min(1f,ȧ/ʩ);if(ʪ<ʠ)ʪ=ʠ;if(ʪ>ʧ)ʪ=ʧ;return ʪ;}internal static float Ɔ(float ʫ,float ƅ,float ʧ){
float ǅ=ʧ>1e-4f?ƅ/ʧ:1f;ǅ=Math.Max(0.88f,ǅ);return ʫ*(0.028f+0.012f*ǅ);}internal static int Ɗ(int Ɖ){int ť=Ɖ-ʡ;return ť<4?Math
.Max(1,Ɖ-1):ť;}internal static int Ɵ(string ú,int Ɖ,int Ƌ){if(string.IsNullOrEmpty(ú))return 0;int ã=0;int ʬ=0;bool ʭ=
true;while(ã<ú.Length){while(ã<ú.Length&&ú[ã]==' ')ã++;if(ã>=ú.Length)break;int ʮ=ʭ?Ɖ:Ƌ;ʭ=false;int ʯ=0;while(ã<ú.Length){
while(ã<ú.Length&&ú[ã]==' ')ã++;if(ã>=ú.Length)break;int ʰ=ã;while(ã<ú.Length&&ú[ã]!=' ')ã++;int ʱ=ã-ʰ;if(ʱ<=0)continue;int ʲ
=ʯ==0?ʱ:(1+ʱ);if(ʯ+ʲ<=ʮ){ʯ+=ʲ;continue;}if(ʯ==0){int ɧ=ʮ<1?1:ʮ;int ʳ=ã;int ʴ=ʰ;while(ʴ<ʳ){int ʵ=Math.Min(ɧ,ʳ-ʴ);ʴ+=ʵ;ʬ++;
ʭ=false;}}else{ã=ʰ;ʬ++;ʭ=false;}goto ʶ;}ʬ++;ʭ=false;ʶ:;}return ʬ;}internal int Ǔ(string ú,float Ǣ,float ʷ,float ʸ,float ʹ
,VRageMath.Color ʺ,string ʻ,TextAlignment ʼ,int Ɖ,int Ƌ,bool ʽ){if(string.IsNullOrEmpty(ú)){ǐ(" ",Ǣ,ʷ,ʹ,ʺ,ʻ,ʼ);return 1;}
int ã=0;int ʬ=0;bool ʭ=true;float Ȥ=ʷ;while(ã<ú.Length){while(ã<ú.Length&&ú[ã]==' ')ã++;if(ã>=ú.Length)break;int ʮ=ʭ?Ɖ:Ƌ;
int ʾ=ã;int ʿ=ã;int ʯ=0;while(ã<ú.Length){while(ã<ú.Length&&ú[ã]==' ')ã++;if(ã>=ú.Length)break;int ʰ=ã;while(ã<ú.Length&&ú[
ã]!=' ')ã++;int ʳ=ã;int ʱ=ʳ-ʰ;if(ʱ<=0)continue;int ʲ=ʯ==0?ʱ:(1+ʱ);if(ʯ+ʲ<=ʮ){ʯ+=ʲ;ʿ=ʳ;continue;}if(ʯ==0){int ʵ=ʮ<1?1:ʮ;ʿ=
ʰ+ʵ;ã=ʿ;}else{ã=ʰ;}break;}string ˀ=ú.Substring(ʾ,Math.Max(0,ʿ-ʾ)).TrimEnd();if(!ʭ&&ʽ&&ˀ.Length>0)ˀ=ʢ+ˀ;if(ˀ.Length==0)ˀ=
" ";ǐ(ˀ,Ǣ,Ȥ,ʹ,ʺ,ʻ,ʼ);Ȥ+=ʸ;ʬ++;ʭ=false;}if(ʬ==0){ǐ(" ",Ǣ,ʷ,ʹ,ʺ,ʻ,ʼ);return 1;}return ʬ;}internal float ˆ(VRageMath.Vector2 g
,VRageMath.RectangleF h,string i,float ʤ){if(F==null)return g.Y*0.06f;F.ā(ȋ);float ƅ=Ƅ(ʤ,h.Width);float ʸ=Ɔ(g.Y,ƅ,ʤ);int
Ɖ=ƈ(h.Width,ƅ);int Ƌ=Ɗ(Ɖ);int ʬ=0;for(int ˁ=0;ˁ<ȋ.Count;ˁ++){string ɗ=ȋ[ˁ];if(string.IsNullOrEmpty(ɗ))continue;if(!string
.IsNullOrEmpty(i)&&ɗ.IndexOf(i,Ƙ.ƙ)<0)continue;if(ʬ>0)ʬ++;int ɲ=0;while(ɲ<=ɗ.Length){int ɳ=ɗ.IndexOf('\n',ɲ);string Ū=ɳ<0
?ɗ.Substring(ɲ):ɗ.Substring(ɲ,ɳ-ɲ);if(Ū.Length==0)ʬ++;else ʬ+=Ɵ(Ū,Ɖ,Ƌ);if(ɳ<0)break;ɲ=ɳ+1;}}if(ʬ==0)ʬ=1;return ʬ*ʸ+g.Y*
0.02f;}internal void ˇ(VRageMath.Vector2 g,VRageMath.RectangleF x,string i,float y,float z,float ª,float ʤ){if(F==null)return
;float ƺ=ˆ(g,x,i,ʤ);if(y+ƺ<=z||y>=ª)return;F.ā(ȋ);float ƅ=Ƅ(ʤ,x.Width);float ʸ=Ɔ(g.Y,ƅ,ʤ);int Ɖ=ƈ(x.Width,ƅ);int Ƌ=Ɗ(Ɖ);
float Ǣ=x.X+x.Width*0.04f;float Ȥ=y;bool ƥ=false;for(int ˁ=0;ˁ<ȋ.Count;ˁ++){string ɗ=ȋ[ˁ];if(string.IsNullOrEmpty(ɗ))continue
;if(!string.IsNullOrEmpty(i)&&ɗ.IndexOf(i,Ƙ.ƙ)<0)continue;ƥ=true;if(Ȥ>y+0.5f)Ȥ+=ʸ;int ɲ=0;while(ɲ<=ɗ.Length){int ɳ=ɗ.
IndexOf('\n',ɲ);string Ū=ɳ<0?ɗ.Substring(ɲ):ɗ.Substring(ɲ,ɳ-ɲ);if(Ū.Length==0){if(Ȥ+ʸ>z&&Ȥ<ª)ǐ(" ",Ǣ,Ȥ,ƅ,Ǒ,Ǖ,TextAlignment.LEFT
);Ȥ+=ʸ;}else{if(Ȥ+ʸ>z&&Ȥ<ª){int Ʒ=Ǔ(Ū,Ǣ,Ȥ,ʸ,ƅ,ǔ,Ǖ,TextAlignment.LEFT,Ɖ,Ƌ,true);Ȥ+=Ʒ*ʸ;}else{int Ʒ=Ɵ(Ū,Ɖ,Ƌ);Ȥ+=Ʒ*ʸ;}}if(ɳ<
0)break;ɲ=ɳ+1;}}if(!ƥ&&Ȥ+ʸ>z&&Ȥ<ª)ǐ("(no matching status)",Ǣ,Ȥ,ƅ,Ǒ,Ǖ,TextAlignment.LEFT);}internal static void Ź(string ŷ
,l Ÿ,Dictionary<string,float>ˈ,Dictionary<string,float>ˉ,List<int>ˊ,List<string>ˋ){if(Ÿ.Ř==null||Ÿ.ř==null||Ÿ.Ś==null){ˈ.
Clear();ˉ.Clear();ˊ.Clear();ˋ.Clear();return;}bool ˌ=string.IsNullOrEmpty(ŷ);bool ˍ=string.Equals(ŷ,"OresIngots",Ƙ.ƙ);bool ˎ=
string.Equals(ŷ,"Components",Ƙ.ƙ);ˈ.Clear();ˉ.Clear();ˊ.Clear();ˋ.Clear();int ˏ=Ÿ.Ř.Length;for(int ã=0;ã<ˏ;ã++){if(Ÿ.ř==null||
Ÿ.Ś==null||Ÿ.ř[ã]<=0.001f)continue;string ː=Ÿ.Ś[ã]??"";string ˑ=Ÿ.Ř[ã]??"";if(ː=="Ore"){float ˠ;ˈ[ˑ]=ˈ.TryGetValue(ˑ,out
ˠ)?ˠ+Ÿ.ř[ã]:Ÿ.ř[ã];}else if(ː=="Ingot"){float ˠ;ˉ[ˑ]=ˉ.TryGetValue(ˑ,out ˠ)?ˠ+Ÿ.ř[ã]:Ÿ.ř[ã];}else{ˊ.Add(ã);}}if(!ˎ){if(ˌ
||ˍ){foreach(var Ā in ˈ.Keys)ˋ.Add(Ā);foreach(var Ā in ˉ.Keys){if(!ˈ.ContainsKey(Ā))ˋ.Add(Ā);}}else{foreach(var Ā in ˈ.
Keys){if(string.Equals(Ā,ŷ,Ƙ.ƙ))ˋ.Add(Ā);}foreach(var Ā in ˉ.Keys){if(ˈ.ContainsKey(Ā))continue;if(string.Equals(Ā,ŷ,Ƙ.ƙ))ˋ.
Add(Ā);}}ˋ.Sort(StringComparer.OrdinalIgnoreCase);}ˊ.Sort((Ȧ,ɗ)=>string.Compare(Ÿ.Ř[Ȧ]??"",Ÿ.Ř[ɗ]??"",Ƙ.ƙ));if(ˍ)ˊ.Clear();
else if(!ˌ&&!ˎ){for(int ˡ=ˊ.Count-1;ˡ>=0;ˡ--){int ơ=ˊ[ˡ];string ˢ=Ÿ.Ř[ơ]??"";if(!string.Equals(ˢ,ŷ,Ƙ.ƙ))ˊ.RemoveAt(ˡ);}}}
sealed class Ⱦ:º{private readonly A ų;public Ⱦ(A f){ų=f;}private static int ˮ(string i,r s){if(s==null)return 0;if(string.
IsNullOrEmpty(i))return 3;int ť=0;string ˣ="Batteries x"+s.Ŕ;string ˤ="Reactors x"+s.ŕ;string ˬ="Engines x"+s.Ŗ;if(ˣ.IndexOf(i,Ƙ.ƙ)>=
0)ť++;if(ˤ.IndexOf(i,Ƙ.ƙ)>=0)ť++;if(ˬ.IndexOf(i,Ƙ.ƙ)>=0)ť++;return ť;}public float v(A f,VRageMath.Vector2 g,VRageMath.
RectangleF h,string i,j k,l m,n o,p q,r s,t u){if(s==null)return g.Y*0.04f;int ŭ=ˮ(i,s);return g.Y*0.035f+ŭ*(g.Y*0.11f)+g.Y*0.02f;
}public void µ(A f,MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,string i,float y,float z,float ª,j k,l
m,n o,p q,r s,t u){if(s==null)return;int ŭ=ˮ(i,s);float Ȩ=g.Y*0.035f+ŭ*(g.Y*0.11f)+g.Y*0.02f;if(y+Ȩ<=z||y>=ª)return;if(ŭ
==0)return;float Ƨ=x.Width;float ƨ=x.X;float Ȫ=ƨ+Ƨ*0.5f;var Ͱ=new VRageMath.Vector2(Ƨ,g.Y);float ͱ=s.Ŏ>1e-6f?s.Ŏ:1f;float
Ͳ=ǃ.Ǆ(s.Ō/ͱ,0f,1f);float ͳ=s.ŏ>1e-6f?s.ŏ:1f;float ʹ=ǃ.Ǆ(s.ő/ͳ,0f,1f);float Ͷ=s.Ő>1e-6f?s.Ő:1f;float ͷ=ǃ.Ǆ(s.Œ/Ͷ,0f,1f);
string ͺ="Batteries x"+s.Ŕ;string ͻ="Reactors x"+s.ŕ;string ͼ="Engines x"+s.Ŗ;var ǉ=new string[ŭ];var Ǌ=new float[ŭ];var ǋ=new
string[ŭ];int ơ=0;if(string.IsNullOrEmpty(i)||ͺ.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=ͺ;Ǌ[ơ]=Ͳ;ǋ[ơ]="OUT:"+s.Ō.ToString("0.0")+" IN:"+s.ŋ.
ToString("0.0");ơ++;}if(string.IsNullOrEmpty(i)||ͻ.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=ͻ;Ǌ[ơ]=ʹ;ǋ[ơ]="OUT:"+s.ő.ToString("0.0");ơ++;}if(
string.IsNullOrEmpty(i)||ͼ.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=ͼ;Ǌ[ơ]=ͷ;ǋ[ơ]="OUT:"+s.Œ.ToString("0.0");ơ++;}ų.ǎ(y,Ͱ,new VRageMath.Vector2
(Ȫ,0f),ǉ,Ǌ,ǋ,new VRageMath.Color(255,0,0,200),true);}}sealed class ȿ:º{private readonly A ų;public ȿ(A f){ų=f;}private
static int Ί(string i,p q){if(q==null)return 0;if(string.IsNullOrEmpty(i))return 4;int ť=0;string ͽ="Total";string Ά=
"Generators x"+q.Ņ;string Έ="Irrigation x"+q.ņ;string Ή="Cargo";if(ͽ.IndexOf(i,Ƙ.ƙ)>=0)ť++;if(Ά.IndexOf(i,Ƙ.ƙ)>=0)ť++;if(Έ.IndexOf(i,Ƙ
.ƙ)>=0)ť++;if(Ή.IndexOf(i,Ƙ.ƙ)>=0)ť++;return ť;}public float v(A f,VRageMath.Vector2 g,VRageMath.RectangleF h,string i,j
k,l m,n o,p q,r s,t u){if(q==null)return g.Y*0.04f;int ŭ=Ί(i,q);return g.Y*0.035f+ŭ*(g.Y*0.11f)+g.Y*0.02f;}public void µ(
A f,MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,string i,float y,float z,float ª,j k,l m,n o,p q,r s,t
u){if(q==null)return;int ŭ=Ί(i,q);float Ȩ=g.Y*0.035f+ŭ*(g.Y*0.11f)+g.Y*0.02f;if(y+Ȩ<=z||y>=ª)return;if(ŭ==0)return;float
Ƨ=x.Width;float ƨ=x.X;float Ȫ=ƨ+Ƨ*0.5f;var Ͱ=new VRageMath.Vector2(Ƨ,g.Y);string Ό="Total";string Ύ="Generators x"+q.Ņ;
string Ώ="Irrigation x"+q.ņ;string ΐ="Cargo";var ǉ=new string[ŭ];var Ǌ=new float[ŭ];var ǋ=new string[ŭ];int ơ=0;if(string.
IsNullOrEmpty(i)||Ό.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=Ό;Ǌ[ơ]=q.ŀ;ǋ[ơ]=ƛ.Ɯ(q.ļ);ơ++;}if(string.IsNullOrEmpty(i)||Ύ.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=Ύ;Ǌ[ơ
]=q.Ł;ǋ[ơ]=ƛ.Ɯ(q.Ľ);ơ++;}if(string.IsNullOrEmpty(i)||Ώ.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=Ώ;Ǌ[ơ]=q.ł;ǋ[ơ]=ƛ.Ɯ(q.ľ);ơ++;}if(string.
IsNullOrEmpty(i)||ΐ.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=ΐ;Ǌ[ơ]=q.Ń;ǋ[ơ]=ƛ.Ɯ(q.Ŀ);ơ++;}ų.ǎ(y,Ͱ,new VRageMath.Vector2(Ȫ,0f),ǉ,Ǌ,ǋ,new VRageMath.
Color(165,220,255,200),true);}}sealed class ɀ:º{private readonly A ų;public ɀ(A f){ų=f;}public float v(A f,VRageMath.Vector2
g,VRageMath.RectangleF h,string i,j k,l m,n o,p q,r s,t u){if(o==null||o.İ==null)return g.Y*0.04f;float Α=g.Y*0.072f;if(
string.IsNullOrEmpty(i)){int Β=o.İ.Length;int Γ=Β>0?(Β+1)/2:1;return g.Y*0.180f+Γ*Α+g.Y*0.02f;}if(string.Equals(i,"Priority",Ƙ
.ƙ))return g.Y*0.180f;int Δ=0;int ť=o.İ.Length;for(int ã=0;ã<ť;ã++){string ˢ=o.İ[ã]??"";if(ˢ.IndexOf(i,Ƙ.ƙ)>=0)Δ++;}int Ε
=Δ>0?(Δ+1)/2:0;return g.Y*0.08f+Ε*Α+g.Y*0.02f;}public void µ(A f,MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.
RectangleF x,string i,float y,float z,float ª,j k,l m,n Ċ,p q,r s,t u){if(Ċ==null||Ċ.İ==null)return;float Α=g.Y*0.072f;float Ζ;if(
string.IsNullOrEmpty(i)){int Η=Ċ.İ.Length;int Γ=Η>0?(Η+1)/2:1;Ζ=g.Y*0.180f+Γ*Α+g.Y*0.02f;}else if(string.Equals(i,"Priority",Ƙ
.ƙ))Ζ=g.Y*0.180f;else{int Θ=0;for(int ˡ=0;ˡ<Ċ.İ.Length;ˡ++){if((Ċ.İ[ˡ]??"").IndexOf(i,Ƙ.ƙ)>=0)Θ++;}int Ε=Θ>0?(Θ+1)/2:0;Ζ=
g.Y*0.08f+Ε*Α+g.Y*0.02f;}if(y+Ζ<=z||y>=ª)return;float Ƨ=x.Width;float ƨ=x.X;float Ȫ=ƨ+Ƨ*0.5f;float Ι=Ƨ*0.5f;const float Κ
=0.52f;const float Λ=0.58f;float Μ=g.Y*0.038f;if(string.Equals(i,"Priority",Ƙ.ƙ)){string Ν=Ċ.ĺ;string Ξ=Ċ.Ļ;if(string.
IsNullOrEmpty(Ν)){Ν="1. Fe  2. Co  3. Ni";Ξ=null;}ų.ǐ(Ν,Ȫ,y+g.Y*0.025f,0.72f,A.ǔ,A.ǒ,TextAlignment.CENTER);if(!string.IsNullOrEmpty(Ξ
))ų.ǐ(Ξ,Ȫ,y+g.Y*0.075f,0.72f,A.ǔ,A.ǒ,TextAlignment.CENTER);return;}if(string.IsNullOrEmpty(i)){string Ν=Ċ.ĺ;string Ξ=Ċ.Ļ;
if(string.IsNullOrEmpty(Ν)){Ν="1. Fe  2. Co  3. Ni";Ξ=null;}ų.ǐ(Ν,Ȫ,y+g.Y*0.025f,0.72f,A.ǔ,A.ǒ,TextAlignment.CENTER);if(!
string.IsNullOrEmpty(Ξ))ų.ǐ(Ξ,Ȫ,y+g.Y*0.075f,0.72f,A.ǔ,A.ǒ,TextAlignment.CENTER);}float Ο=string.IsNullOrEmpty(i)?y+g.Y*0.180f
:y+g.Y*0.08f;int Β=Ċ.İ.Length;int Π=0;for(int ã=0;ã<Β;ã++){if(!string.IsNullOrEmpty(i)){string Ρ=Ċ.İ[ã]??"";if(Ρ.IndexOf(
i,Ƙ.ƙ)<0)continue;}int Σ=Π%2;int Τ=Π/2;Π++;float Υ=ƨ+Σ*Ι;float Φ=Ο+Τ*Α;float Χ=Φ-g.Y*0.018f;float Ψ=Υ+Ι*0.065f;string Ω=Ċ
.İ[ã]??"Unknown Refinery";bool ķ=(Ċ.ķ!=null&&ã<Ċ.ķ.Length)?Ċ.ķ[ã]:false;bool Ĺ=(Ċ.Ĺ!=null&&ã<Ċ.Ĺ.Length)?Ċ.Ĺ[ã]:false;
string Ϊ=(Ċ.Ĳ!=null&&ã<Ċ.Ĳ.Length)?Ċ.Ĳ[ã]:"";var Ϋ=A.Ǒ;if(ķ)Ϋ=A.Ǩ;else if(Ĺ)Ϋ=A.ǧ;string έ=Ĺ&&!string.IsNullOrEmpty(Ϊ)?ƛ.ά(Ϊ):
"-";ų.ǐ(έ,Υ+Ι*0.24f,Χ,Κ,new VRageMath.Color(220,220,220,255),A.Ǖ,TextAlignment.CENTER);ų.ǐ(Ω,Υ+Ι*0.36f,Χ,Λ,A.ǔ,A.ǒ,
TextAlignment.LEFT);ų.ȩ("Circle",Ψ,Φ,Μ,Μ,Ϋ);}}}sealed class Ɂ:º{private readonly A ų;public Ɂ(A f){ų=f;}public float v(A f,VRageMath.
Vector2 g,VRageMath.RectangleF h,string i,j k,l m,n o,p q,r s,t u){if(u==null||u.Ţ)return g.Y*0.22f;int ʬ=0;if(u.ŗ)ʬ++;if(u.ś)ʬ
++;if(u.Ň)ʬ++;if(u.ŝ)ʬ++;if(u.Ş)ʬ++;if(u.Ŝ)ʬ++;if(ʬ==0)ʬ=1;return ʬ*(g.Y*0.065f)+g.Y*0.02f;}public void µ(A f,
MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,string i,float y,float z,float ª,j k,l m,n o,p q,r s,t u){int ʬ=0;if(u!=
null&&!u.Ţ){if(u.ŗ)ʬ++;if(u.ś)ʬ++;if(u.Ň)ʬ++;if(u.ŝ)ʬ++;if(u.Ş)ʬ++;if(u.Ŝ)ʬ++;if(ʬ==0)ʬ=1;}float Ζ=u==null||u.Ţ?g.Y*0.22f:ʬ*
(g.Y*0.065f)+g.Y*0.02f;if(y+Ζ<=z||y>=ª)return;if(u==null)return;float Ƨ=x.Width;float ƨ=x.X;float Ȫ=ƨ+Ƨ*0.5f;float ʤ=Math
.Min(1f,Ƨ/350f);if(u.Ţ){ų.ǐ("ALL SYSTEMS NOMINAL",Ȫ,y+g.Y*0.13f,1.0f*ʤ,A.Ǩ,A.ǒ,TextAlignment.CENTER);return;}ų.Ȋ.Clear();
if(u.ŗ)ų.Ȋ.Add("LOW POWER");if(u.ś)ų.Ȋ.Add("CARGO FULL");if(u.Ň)ų.Ȋ.Add("LOW ICE");if(u.ŝ)ų.Ȋ.Add("REFINERY STALLED");if(u
.Ş)ų.Ȋ.Add("ASSEMBLER STALLED");if(u.Ŝ)ų.Ȋ.Add("NO REFINERIES");float Ȥ=y+g.Y*0.02f;float ή=g.Y*0.065f;for(int ã=0;ã<ų.Ȋ.
Count;ã++){string ȧ=ų.Ȋ[ã];ų.ǐ(ȧ,Ȫ,Ȥ,0.92f*ʤ,A.ǧ,A.ǒ,TextAlignment.CENTER);Ȥ+=ή;}}}sealed class ɂ:º{private readonly A ų;
private const float ί=0.52f;public ɂ(A f){ų=f;}public float v(A f,VRageMath.Vector2 g,VRageMath.RectangleF h,string i,j k,l m,n
o,p q,r s,t u){return ų.ˆ(g,h,i??"",ί);}public void µ(A f,MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,
string i,float y,float z,float ª,j k,l m,n o,p q,r s,t u){float Ζ=ų.ˆ(g,x,i??"",ί);if(y+Ζ<=z||y>=ª)return;ų.ˇ(g,x,i??"",y,z,ª,
ί);}}}public sealed class ȇ{public const string ɇ="COL";float ΰ,α;float β,γ,δ,ε;int ζ;public VRageMath.RectangleF ʍ{get;
private set;}public void ʂ(float η,float θ){ΰ=η;β=θ;α=0f;γ=0f;δ=0f;ε=0f;ζ=0;ʍ=new VRageMath.RectangleF(0f,0f,ΰ,β);}public float
ʋ{get{if(ζ==0)return α;return Math.Max(α,γ+Math.Max(δ,ε));}}public float ʙ{get{if(ζ==0)return α;if(ζ==1)return γ+δ;return
γ+ε;}}public float ʝ{get{return ʍ.X+ʍ.Width*0.5f;}}public void ʄ(string ι){string Ȧ=ι==null?"":ι.Trim();if(Ȧ.Length==0){κ
();return;}if(string.Equals(Ȧ,"FULL",Ƙ.ƙ)){λ();return;}if(string.Equals(Ȧ,"LEFT",Ƙ.ƙ)){μ();return;}if(string.Equals(Ȧ,
"RIGHT",Ƙ.ƙ)){ν();return;}}void κ(){if(ζ==0||ζ==2){ξ();γ=α;δ=0f;ε=0f;ζ=1;ο(1);return;}if(ζ==1){ζ=2;ο(2);}}void μ(){if(ζ==1||ζ==
2)ξ();γ=α;δ=0f;ε=0f;ζ=1;ο(1);}void ν(){if(ζ==0){γ=α;δ=0f;ε=0f;}else if(ζ==1){}else if(ζ==2){ξ();γ=α;δ=0f;ε=0f;}ζ=2;ο(2);}
public void λ(){ξ();ζ=0;ʍ=new VRageMath.RectangleF(0f,0f,ΰ,β);}public void ʉ(float π){if(ζ==0){α+=π;return;}if(ζ==1)δ+=π;else
ε+=π;}public void ʊ(){ξ();}void ξ(){if(ζ==0)return;float ρ=γ+Math.Max(δ,ε);if(ρ>α)α=ρ;ζ=0;δ=0f;ε=0f;ʍ=new VRageMath.
RectangleF(0f,0f,ΰ,β);}void ο(int Σ){float ς=ΰ*0.5f;if(Σ==1)ʍ=new VRageMath.RectangleF(0f,0f,ς,β);else ʍ=new VRageMath.RectangleF(
ς,0f,ς,β);}}public static class Ĉ{public static ċ ĉ<ċ>(string ù){try{if(ù==null)return default(ċ);return Ų.ĉ<ċ>(ù);}catch
{}return default(ċ);}}public class C{}public class p{public float ļ,Ľ,ľ,Ŀ,ŀ,Ł,ł,Ń;public int Ņ,ņ;public bool Ň;}public
class l{public string[]Ř,Ś;public float[]ř;}public class j{public float ĕ,Ė,ė,Ę,ę,Ě,ě,Ĝ,ĝ,Ğ,ğ,Ġ,ġ,Ģ,ģ,Ĥ,ĥ,Ħ,ħ,Ĩ,ĩ,Ī,ī,Ĭ,ĭ,Į,į
;}public class r{public float ŉ,Ŋ,ŋ,Ō,ō,Ŏ,ő,Œ,ŏ,Ő;public int Ŕ,ŕ,Ŗ;public bool ŗ;}public class n{public string[]İ,Ĳ,ĵ;
public float[]ĳ,Ķ;public bool[]ķ,Ĺ;public string ĺ,Ļ;}public class t{public bool Ň,ŗ,ś,Ŝ,ŝ,Ş,Ţ;public int Š;public string š;}
public static class Ô{public const string Û="SYS_STATUS",Ü="PB1_WARNINGS",Õ="PB1ToPB2_InventorySummary",Ö=
"PB1ToPB2_RefineryStatus",Ø="PB1ToPB2_IceStatus",Ù="PB1ToPB2_PowerStatus",Ú="PB1ToPB2_InventoryDynamic";}public static class ô{private const uint
σ=2166136261u,τ=16777619u;public const long ö=90L*TimeSpan.TicksPerSecond;public static bool ć(string υ,string φ,
Dictionary<string,long>χ,Dictionary<string,long>ψ,long Ă,long ω,out string T,out string ϊ){T=null;ϊ=null;if(υ==null||χ==null){
return false;}string[]Ŧ=υ.Split(new[]{'|'},4);if(Ŧ.Length!=4){return false;}string ϋ=Ŧ[0];string ό=Ŧ[1];string ύ=Ŧ[2];string ώ
=Ŧ[3];if(ϋ==null||ό==null||ύ==null||ώ==null){return false;}string Ϗ;if(ύ.Length==0){Ϗ="";}else{byte[]Ý;try{Ý=Convert.
FromBase64String(ύ);}catch{return false;}Ϗ=Encoding.UTF8.GetString(Ý);}long ϐ;if(!long.TryParse(ό,out ϐ)){return false;}long ϑ=0;bool ϒ=
ψ!=null&&ω>0;long ˠ;if(χ.TryGetValue(ϋ,out ˠ)){if(ϒ){long ϓ;if(ψ.TryGetValue(ϋ,out ϓ)){if(Ă-ϓ>ω){χ.Remove(ϋ);ψ.Remove(ϋ);
ϑ=0;}else{ϑ=ˠ;}}else{χ.Remove(ϋ);ϑ=0;}}else{ϑ=ˠ;}}if(ϐ<=ϑ){return false;}string ϔ=φ??"";uint Ȩ=σ;Ȩ=ϕ(Ȩ,ϋ);Ȩ=ϕ(Ȩ,ό);Ȩ=ϕ(Ȩ,
Ϗ);Ȩ=ϕ(Ȩ,ϔ);string ϖ=Ȩ.ToString("X8");if(!string.Equals(ώ,ϖ,StringComparison.Ordinal)){return false;}χ[ϋ]=ϐ;if(ϒ){ψ[ϋ]=Ă;
}T=ϋ;ϊ=Ϗ;return true;}public static void õ(Dictionary<string,long>χ,Dictionary<string,long>ψ,long Ă,long ω,List<string>ϗ)
{if(χ==null||ψ==null||ϗ==null){return;}if(ω<=0){return;}ϗ.Clear();foreach(KeyValuePair<string,long>ÿ in ψ){if(Ă-ÿ.Value>ω
){ϗ.Add(ÿ.Key);}}for(int ã=0;ã<ϗ.Count;ã++){string Ā=ϗ[ã];χ.Remove(Ā);ψ.Remove(Ā);}ϗ.Clear();foreach(string Ā in χ.Keys){
if(!ψ.ContainsKey(Ā)){ϗ.Add(Ā);}}for(int ã=0;ã<ϗ.Count;ã++){χ.Remove(ϗ[ã]);}}private static uint ϕ(uint Ϙ,string ţ){if(ţ==
null||ţ.Length==0){return Ϙ;}for(int ã=0;ã<ţ.Length;ã++){char ű=ţ[ã];Ϙ^=(byte)(ű&0xFF);Ϙ*=τ;Ϙ^=(byte)((ű>>8)&0xFF);Ϙ*=τ;}
return Ϙ;}}public static class ɬ{public static bool ɭ(string Ƣ,string ă){if(string.IsNullOrEmpty(Ƣ)||string.IsNullOrEmpty(ă))
return false;return Ƣ.IndexOf(ă,StringComparison.OrdinalIgnoreCase)>=0;}}public static class ƛ{private static readonly
StringBuilder ϙ=new StringBuilder(48);public static string ǆ(float Ϛ){if(float.IsNaN(Ϛ))return"NaN%";if(float.IsInfinity(Ϛ))return Ϛ>
0f?"Infinity%":"-Infinity%";int ē=(int)Math.Round((double)Ϛ);ϙ.Clear();ϙ.Append(ē.ToString());ϙ.Append('%');return ϙ.
ToString();}public static string Ɯ(float Ϛ){if(float.IsNaN(Ϛ))return"NaN";if(float.IsInfinity(Ϛ))return Ϛ>0f?"Infinity":
"-Infinity";bool ϛ=Ϛ<0f;double Ϝ=ϛ?-(double)Ϛ:(double)Ϛ;string è="";double ϝ=1.0;if(Ϝ>=1e9){è="B";ϝ=1e9;}else if(Ϝ>=1e6){è="M";ϝ=
1e6;}else if(Ϝ>=1e3){è="k";ϝ=1e3;}ϙ.Clear();if(ϛ)ϙ.Append('-');if(è.Length>0){double ʪ=Ϝ/ϝ;ʪ=Math.Round(ʪ*10.0)/10.0;ϙ.
Append(ʪ.ToString("0.0"));ϙ.Append(è);}else{float Ϟ=ϛ?-(float)Ϝ:(float)Ϝ;ϙ.Append(Ϟ.ToString("0.######"));}return ϙ.ToString()
;}public static string ά(string ϟ){if(string.IsNullOrEmpty(ϟ)){return"-";}if(string.Equals(ϟ,"Iron",Ƙ.ƙ)){return"Fe";}if(
string.Equals(ϟ,"Nickel",Ƙ.ƙ)){return"Ni";}if(string.Equals(ϟ,"Cobalt",Ƙ.ƙ)){return"Co";}if(string.Equals(ϟ,"Silicon",Ƙ.ƙ)){
return"Si";}if(string.Equals(ϟ,"Silver",Ƙ.ƙ)){return"Ag";}if(string.Equals(ϟ,"Gold",Ƙ.ƙ)){return"Au";}if(string.Equals(ϟ,
"Magnesium",Ƙ.ƙ)){return"Mg";}if(string.Equals(ϟ,"Platinum",Ƙ.ƙ)){return"Pt";}if(string.Equals(ϟ,"Uranium",Ƙ.ƙ)){return"U";}if(
string.Equals(ϟ,"Stone",Ƙ.ƙ)){return"St";}if(string.Equals(ϟ,"Ice",Ƙ.ƙ)){return"Ic";}if(ϟ.Length<=2){return ϟ.ToUpperInvariant
();}return ϟ.Substring(0,2).ToUpperInvariant();}}public static class ǃ{public static float Ǆ(float Ϛ,float Ϡ,float ǈ){if(
Ϡ>ǈ){float ϡ=Ϡ;Ϡ=ǈ;ǈ=ϡ;}if(Ϛ<Ϡ)return Ϡ;if(Ϛ>ǈ)return ǈ;return Ϛ;}}public static class Ƙ{public const StringComparison ƙ=
StringComparison.OrdinalIgnoreCase;