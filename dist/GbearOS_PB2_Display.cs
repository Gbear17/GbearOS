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
IMyGridTerminalSystem Ȼ,IMyProgrammableBlock Ñ,C ȼ,E Ƚ){Ȃ=Ȼ;À=Ñ;D=ȼ;F=Ƚ;ȓ=0;Ⱦ();}º Ɂ(string ȿ){if(ȝ==null||ȿ==null)return null;º ɀ;return ȝ.
TryGetValue(ȿ,out ɀ)?ɀ:null;}public void W(double ɂ){if(Ȃ==null||À==null)return;Ƀ();string Ʉ="Offline for: "+ɂ.ToString("F0")+"s";
int ŭ=Ȅ.Count;for(int ã=0;ã<ŭ;ã++){IMyTextPanel Ʌ=Ȅ[ã].ǹ;if(Ʌ==null)continue;Ȟ(Ʌ);VRageMath.Vector2 ȟ;VRageMath.Vector2 Ƞ;Ɇ
(Ʌ,out ȟ,out Ƞ);using(var w=Ʌ.DrawFrame()){ȡ(w,ȟ,Ƞ);Ȇ.Clear();ǐ("NO SIGNAL",Ƞ.X,ȟ.Y*0.10f,1.35f,ǧ,ǒ,TextAlignment.CENTER)
;ǐ("WAITING FOR TELEMETRY...",Ƞ.X,ȟ.Y*0.20f,0.72f,ǔ,ǒ,TextAlignment.CENTER);ǐ(Ʉ,Ƞ.X,ȟ.Y*0.28f,0.62f,Ǒ,ǒ,TextAlignment.
CENTER);ɇ(w);}}}public void X(j k,n o,p q,r s,l m,t u,bool O){if(Ȃ==null||À==null)return;Ƀ();if(O){Ȝ=Ɉ(Ȗ,k);Ȏ=Ɉ(ȗ,o);ȏ=Ɉ(Ș,q);
Ȑ=Ɉ(ș,s);ȑ=Ɉ(Ț,m);Ȓ=Ɉ(ț,u);ȍ=ɉ();Ȗ=k;ȗ=o;Ș=q;ș=s;Ț=m;ț=u;}Ɋ(k,o,q,s,m,u);ɋ(k,o,q,s,m,u);}private static bool Ɉ<ċ>(ċ Ȧ,ċ Ɍ
){if(Ȧ==null&&Ɍ==null)return false;if(Ȧ==null||Ɍ==null)return true;return!Ȧ.Equals(Ɍ);}bool ɐ(List<Ǹ>ɍ){if(ɍ==null||ɍ.
Count==0)return false;bool Ɏ=Ȝ||ȑ||Ȏ||ȏ||Ȑ||Ȓ||ȍ;bool ɏ=Ȝ||ȑ;int ť=ɍ.Count;for(int ã=0;ã<ť;ã++){switch(ɍ[ã].ǵ){case Ǵ.ǭ:if(ɏ)
return true;break;case Ǵ.Ǯ:if(Ȏ)return true;break;case Ǵ.ǰ:if(ȏ)return true;break;case Ǵ.ǯ:if(Ȑ)return true;break;case Ǵ.Ǳ:if(
Ȓ)return true;break;case Ǵ.ǲ:if(ȍ)return true;break;case Ǵ.ǫ:if(Ɏ)return true;break;}}return false;}void ɇ(
MySpriteDrawFrame w){int ť=Ȇ.Count;for(int ɑ=0;ɑ<ť;ɑ++)w.Add(Ȇ[ɑ]);Ȇ.Clear();}void ɋ(j k,n o,p q,r s,l m,t u){int ŭ=Ȅ.Count;for(int ã=0;ã
<ŭ;ã++){var M=Ȅ[ã];if(M.Ǻ==null||M.Ǻ.Count==0)continue;if(!ɐ(M.Ǻ)&&!M.Ȁ)continue;ɒ(ref M,k,o,q,s,m,u);M.Ȁ=false;Ȅ[ã]=M;}}
void Ɋ(j k,n o,p q,r s,l m,t u){int ŭ=Ȅ.Count;for(int ã=0;ã<ŭ;ã++){var M=Ȅ[ã];if(M.Ǻ==null)continue;VRageMath.Vector2 ɓ,ɔ;Ɇ(
M.ǹ,out ɓ,out ɔ);float ɕ=ɓ.Y*0.95703125f;float ɖ,ɗ;ɘ(Ȉ,M.Ǻ,ɓ,k,o,q,s,m,u,out ɖ,out ɗ);M.ǽ=ɖ;M.Ǿ=ɗ;float ə=ɕ-ɖ;if(ɗ>ə){
float ɚ=ɗ-ə;float ɛ=ə*0.90f;if(M.Ǽ>M.ǻ){float ɜ=ɛ/12f;M.ǻ+=ɜ;if(M.ǻ>=M.Ǽ)M.ǻ=M.Ǽ;M.Ȁ=true;}else if(M.Ǽ<M.ǻ){float ɝ=M.ǻ-M.Ǽ;
float ɞ=ɝ*0.15f;if(ɞ<20f)ɞ=20f;M.ǻ-=ɞ;if(M.ǻ<=M.Ǽ)M.ǻ=M.Ǽ;M.Ȁ=true;}else{M.ǿ++;if(M.ǿ>=30){M.ǿ=0;if(M.ǻ>=ɚ-5f){M.Ǽ=0f;}else{M
.Ǽ=M.ǻ+ɛ;if(M.Ǽ>ɚ)M.Ǽ=ɚ;}M.Ȁ=true;}}}else{M.ǻ=0f;M.Ǽ=0f;M.ǿ=0;}Ȅ[ã]=M;}}void Ƀ(){if(ȓ>0){ȓ--;return;}ȓ=100;ȃ.Clear();Ȃ.
GetBlocksOfType(ȃ,ɟ);ȅ.Clear();for(int ɠ=0;ɠ<Ȅ.Count;ɠ++)ȅ.Add(Ȅ[ɠ]);Ȅ.Clear();int ť=ȃ.Count;for(int ã=0;ã<ť;ã++){var ē=ȃ[ã];if(ē==null
)continue;string Ƣ=ē.CustomName;if(ɡ.ɢ(Ƣ,ǥ))continue;ȁ M;M.ǹ=ē;M.ǻ=0f;M.Ǽ=0f;M.ǿ=0;M.Ȁ=false;M.ǽ=0f;M.Ǿ=0f;for(int ɣ=0;ɣ<
ȅ.Count;ɣ++){if(ȅ[ɣ].ǹ==ē){M.ǻ=ȅ[ɣ].ǻ;M.Ǽ=ȅ[ɣ].Ǽ;M.ǿ=ȅ[ɣ].ǿ;break;}}if(!ɡ.ɢ(Ƣ,Ǥ))continue;var ɍ=new List<Ǹ>(8);ɤ(ē.
CustomData,ɍ);if(ɍ.Count==0)continue;M.Ǻ=ɍ;Ȅ.Add(M);}}void ɤ(string ɥ,List<Ǹ>ü){ü.Clear();bool ɦ=string.IsNullOrWhiteSpace(ɥ);if(ɦ
){ü.Add(new Ǹ{ǵ=Ǵ.ǭ,Ƕ="",Ƿ=null});return;}int ɧ=0;int â=ɥ.Length;while(ɧ<â){int ɨ=ɥ.IndexOf('\n',ɧ);string ɩ=ɨ<0?ɥ.
Substring(ɧ):ɥ.Substring(ɧ,ɨ-ɧ);ɧ=ɨ<0?â:ɨ+1;int ǉ=ɩ.IndexOf('[');int ɪ=ɩ.IndexOf(']');if(ǉ<0||ɪ<=ǉ)continue;string ɫ=ɩ.Substring(
ǉ+1,ɪ-ǉ-1).Trim();if(ɫ.Length==0)continue;Ǹ ɬ;int ű=ɫ.IndexOf(':');string ɭ;if(ű<0){ɭ=ɫ.Trim();ɬ.Ƕ="";}else{ɭ=ɫ.Substring
(0,ű).Trim();ɬ.Ƕ=ɫ.Substring(ű+1).Trim();}if(ɭ.Length==0)continue;ɬ.ǵ=ɮ(ɭ);if(ɬ.ǵ==Ǵ.ǫ)ɬ.Ƿ=ɭ;else ɬ.Ƿ=null;ü.Add(ɬ);}}
bool ɟ(IMyTextPanel ē){if(ē==null)return false;if(!ē.IsSameConstructAs(À))return false;return true;}private static void Ɇ(
IMyTextPanel Ʌ,out VRageMath.Vector2 ȟ,out VRageMath.Vector2 Ƞ){var ɯ=Ʌ as IMyTextSurface;var ɰ=ɯ!=null?ɯ.TextureSize:default(
VRageMath.Vector2);var ɱ=ɯ!=null?ɯ.SurfaceSize:default(VRageMath.Vector2);ȟ=(ɰ.X>=8f&&ɰ.Y>=8f)?ɰ:((ɱ.X>=8f&&ɱ.Y>=8f)?ɱ:new
VRageMath.Vector2(512f,512f));Ƞ=ȟ*0.5f;}float ɲ(VRageMath.Vector2 ȟ){return ȟ.Y*0.045f;}float ɵ(float ɳ,VRageMath.Vector2 ȟ,float
Ȫ,string ɴ,bool ȶ){float Ȩ=ɲ(ȟ);if(ȶ)ǐ("--- "+ɴ+" ---",Ȫ,ɳ,0.55f,Ǒ,ǒ,TextAlignment.CENTER);return Ȩ;}void ɘ(ȇ ɶ,List<Ǹ>ɍ,
VRageMath.Vector2 ȟ,j k,n o,p q,r s,l m,t u,out float Ƹ,out float ɷ){Ƹ=ȟ.Y*0.02f;ɶ.ɸ(ȟ.X,ȟ.Y);int ɹ=ɍ.Count;for(int ã=0;ã<ɹ;ã++){
var ű=ɍ[ã];switch(ű.ǵ){case Ǵ.Ǭ:Ƹ+=ȟ.Y*0.07f;continue;case Ǵ.ǳ:ɶ.ɺ(ű.Ƕ);continue;}bool ɻ=(ű.ǵ==Ǵ.ǭ||ű.ǵ==Ǵ.ǲ)&&!string.
IsNullOrEmpty(ű.Ƕ);float ɼ=ɻ?0f:ɲ(ȟ);float ɾ=ɽ(ű,ɶ,ȟ,k,o,q,s,m,u);ɶ.ɿ(ɼ+ɾ);}ɶ.ʀ();ɷ=ɶ.ʁ;}float ɽ(Ǹ ʂ,ȇ ɶ,VRageMath.Vector2 ȟ,j k,n o,
p q,r s,l m,t u){if(ʂ.ǵ==Ǵ.ǳ)return 0f;string ʄ=ʃ(ref ʂ);º ɀ=Ɂ(ʄ);if(ɀ!=null)return ɀ.v(this,ȟ,ɶ.ʅ,ʂ.Ƕ,k,m,o,q,s,u);
return ȟ.Y*0.04f;}void ɒ(ref ȁ ʆ,j k,n o,p q,r s,l m,t u){IMyTextPanel Ʌ=ʆ.ǹ;if(Ʌ==null)return;Ȟ(Ʌ);VRageMath.Vector2 ȟ;
VRageMath.Vector2 Ƞ;Ɇ(Ʌ,out ȟ,out Ƞ);float ƹ=ȟ.Y*0.95703125f;float Ƹ=ʆ.ǽ;float ʇ=ʆ.Ǿ;float ʈ=ƹ-Ƹ;float ʉ=ȟ.Y*0.02f;float ʊ=Ƹ+ʉ-ʆ.
ǻ;using(var w=Ʌ.DrawFrame()){ȡ(w,ȟ,Ƞ);Ȇ.Clear();float ʋ=ȟ.Y*0.025f;int ɹ=ʆ.Ǻ.Count;for(int ã=0;ã<ɹ;ã++){var ű=ʆ.Ǻ[ã];if(ű
.ǵ!=Ǵ.Ǭ)continue;string ʌ=string.IsNullOrEmpty(ű.Ƕ)?" ":ű.Ƕ;ǐ(ʌ,Ƞ.X,ʋ,0.88f,ǔ,ǒ,TextAlignment.CENTER);ʋ+=ȟ.Y*0.07f;}if(ʇ>
ʈ){float ʍ=ʇ-ʈ;float ʎ=ʈ*0.90f;int ʏ=(int)Math.Ceiling(ʍ/ʎ)+1;int ʐ;if(ʆ.ǻ>=ʍ-5f)ʐ=ʏ;else ʐ=(int)(ʆ.ǻ/ʎ)+1;ǐ("PAGE "+ʐ+
"/"+ʏ,ȟ.X*0.97f,ȟ.Y*0.025f,0.5f,new VRageMath.Color(180,180,180,255),ǒ,TextAlignment.RIGHT);}Ȕ=Ƹ+ʉ;ȕ=ƹ;ȉ.ɸ(ȟ.X,ȟ.Y);for(int
ã=0;ã<ɹ;ã++){var ű=ʆ.Ǻ[ã];switch(ű.ǵ){case Ǵ.Ǭ:continue;case Ǵ.ǳ:ȉ.ɺ(ű.Ƕ);continue;}bool ɻ=(ű.ǵ==Ǵ.ǭ||ű.ǵ==Ǵ.ǲ)&&!string.
IsNullOrEmpty(ű.Ƕ);float ɼ=ɻ?0f:ɲ(ȟ);float ɾ=ɽ(ű,ȉ,ȟ,k,o,q,s,m,u);float ʒ=ʊ+ȉ.ʑ;float ʓ=ʒ+ɼ+ɾ;bool ʔ=ʓ<=Ƹ||ʒ>=ƹ;if(!ʔ){if(!ɻ)ɵ(ʒ,ȟ,ȉ.
ʕ,ʖ(ű.ǵ,ű.Ƿ),true);float Ⱥ=ʒ+ɼ;ʗ(ű,ȉ,w,k,o,q,s,m,u,ȟ,Ⱥ,Ƹ,ƹ);}ȉ.ɿ(ɼ+ɾ);}ȉ.ʀ();Ȕ=-1f;ɇ(w);}}void ʗ(Ǹ ʂ,ȇ ɶ,
MySpriteDrawFrame w,j k,n o,p q,r s,l m,t u,VRageMath.Vector2 ȟ,float y,float Ƹ,float ƹ){string ʄ=ʃ(ref ʂ);º ɀ=Ɂ(ʄ);if(ɀ!=null){ɀ.µ(this,
w,ȟ,ɶ.ʅ,ʂ.Ƕ,y,Ƹ,ƹ,k,m,o,q,s,u);}}bool ɉ(){if(F==null)return false;F.ā(ȋ);bool ʘ=ȋ.Count!=Ȍ.Count;if(!ʘ){for(int ã=0;ã<ȋ.
Count;ã++){string Ȧ=ȋ[ã]??"";string Ɍ=ã<Ȍ.Count?(Ȍ[ã]??""):"";if(!string.Equals(Ȧ,Ɍ,Ƙ.ƙ)){ʘ=true;break;}}}if(!ʘ)return false;
Ȍ.Clear();for(int ã=0;ã<ȋ.Count;ã++)Ȍ.Add(ȋ[ã]??"");return true;}private const float ʙ=0.45f;private const int ʚ=2;
private const string ʛ="  ";internal static int ƈ(float ʜ,float ʝ){float ʞ=ʜ*0.80f;if(ʞ<8f)ʞ=Math.Max(1f,ʜ*0.5f);float ʟ=19.5f*
ʝ;if(ʟ<=0.0001f)return 4;int ť=(int)(ʞ/ʟ);return ť<1?1:ť;}internal static float Ƅ(float ʠ,float ʡ){float ȧ=ʡ>2f?ʡ:400f;
float ʢ=520f;float ʣ=ʠ*Math.Min(1f,ȧ/ʢ);if(ʣ<ʙ)ʣ=ʙ;if(ʣ>ʠ)ʣ=ʠ;return ʣ;}internal static float Ɔ(float ʤ,float ƅ,float ʠ){
float ǅ=ʠ>1e-4f?ƅ/ʠ:1f;ǅ=Math.Max(0.88f,ǅ);return ʤ*(0.028f+0.012f*ǅ);}internal static int Ɗ(int Ɖ){int ť=Ɖ-ʚ;return ť<4?Math
.Max(1,Ɖ-1):ť;}internal static int Ɵ(string ú,int Ɖ,int Ƌ){if(string.IsNullOrEmpty(ú))return 0;int ã=0;int ʥ=0;bool ʦ=
true;while(ã<ú.Length){while(ã<ú.Length&&ú[ã]==' ')ã++;if(ã>=ú.Length)break;int ʧ=ʦ?Ɖ:Ƌ;ʦ=false;int ʨ=0;while(ã<ú.Length){
while(ã<ú.Length&&ú[ã]==' ')ã++;if(ã>=ú.Length)break;int ʩ=ã;while(ã<ú.Length&&ú[ã]!=' ')ã++;int ʪ=ã-ʩ;if(ʪ<=0)continue;int ʫ
=ʨ==0?ʪ:(1+ʪ);if(ʨ+ʫ<=ʧ){ʨ+=ʫ;continue;}if(ʨ==0){int ɜ=ʧ<1?1:ʧ;int ʬ=ã;int ʭ=ʩ;while(ʭ<ʬ){int ʮ=Math.Min(ɜ,ʬ-ʭ);ʭ+=ʮ;ʥ++;
ʦ=false;}}else{ã=ʩ;ʥ++;ʦ=false;}goto ʯ;}ʥ++;ʦ=false;ʯ:;}return ʥ;}internal int Ǔ(string ú,float Ǣ,float ʰ,float ʱ,float ʲ
,VRageMath.Color ʳ,string ʴ,TextAlignment ʵ,int Ɖ,int Ƌ,bool ʶ){if(string.IsNullOrEmpty(ú)){ǐ(" ",Ǣ,ʰ,ʲ,ʳ,ʴ,ʵ);return 1;}
int ã=0;int ʥ=0;bool ʦ=true;float Ȥ=ʰ;while(ã<ú.Length){while(ã<ú.Length&&ú[ã]==' ')ã++;if(ã>=ú.Length)break;int ʧ=ʦ?Ɖ:Ƌ;
int ʷ=ã;int ʸ=ã;int ʨ=0;while(ã<ú.Length){while(ã<ú.Length&&ú[ã]==' ')ã++;if(ã>=ú.Length)break;int ʩ=ã;while(ã<ú.Length&&ú[
ã]!=' ')ã++;int ʬ=ã;int ʪ=ʬ-ʩ;if(ʪ<=0)continue;int ʫ=ʨ==0?ʪ:(1+ʪ);if(ʨ+ʫ<=ʧ){ʨ+=ʫ;ʸ=ʬ;continue;}if(ʨ==0){int ʮ=ʧ<1?1:ʧ;ʸ=
ʩ+ʮ;ã=ʸ;}else{ã=ʩ;}break;}string ʹ=ú.Substring(ʷ,Math.Max(0,ʸ-ʷ)).TrimEnd();if(!ʦ&&ʶ&&ʹ.Length>0)ʹ=ʛ+ʹ;if(ʹ.Length==0)ʹ=
" ";ǐ(ʹ,Ǣ,Ȥ,ʲ,ʳ,ʴ,ʵ);Ȥ+=ʱ;ʥ++;ʦ=false;}if(ʥ==0){ǐ(" ",Ǣ,ʰ,ʲ,ʳ,ʴ,ʵ);return 1;}return ʥ;}internal float ʻ(VRageMath.Vector2 g
,VRageMath.RectangleF h,string i,float ʝ){if(F==null)return g.Y*0.06f;F.ā(ȋ);float ƅ=Ƅ(ʝ,h.Width);float ʱ=Ɔ(g.Y,ƅ,ʝ);int
Ɖ=ƈ(h.Width,ƅ);int Ƌ=Ɗ(Ɖ);int ʥ=0;for(int ʺ=0;ʺ<ȋ.Count;ʺ++){string Ɍ=ȋ[ʺ];if(string.IsNullOrEmpty(Ɍ))continue;if(!string
.IsNullOrEmpty(i)&&Ɍ.IndexOf(i,Ƙ.ƙ)<0)continue;if(ʥ>0)ʥ++;int ɧ=0;while(ɧ<=Ɍ.Length){int ɨ=Ɍ.IndexOf('\n',ɧ);string Ū=ɨ<0
?Ɍ.Substring(ɧ):Ɍ.Substring(ɧ,ɨ-ɧ);if(Ū.Length==0)ʥ++;else ʥ+=Ɵ(Ū,Ɖ,Ƌ);if(ɨ<0)break;ɧ=ɨ+1;}}if(ʥ==0)ʥ=1;return ʥ*ʱ+g.Y*
0.02f;}internal void ʼ(VRageMath.Vector2 g,VRageMath.RectangleF x,string i,float y,float z,float ª,float ʝ){if(F==null)return
;float ƺ=ʻ(g,x,i,ʝ);if(y+ƺ<=z||y>=ª)return;F.ā(ȋ);float ƅ=Ƅ(ʝ,x.Width);float ʱ=Ɔ(g.Y,ƅ,ʝ);int Ɖ=ƈ(x.Width,ƅ);int Ƌ=Ɗ(Ɖ);
float Ǣ=x.X+x.Width*0.04f;float Ȥ=y;bool ƥ=false;for(int ʺ=0;ʺ<ȋ.Count;ʺ++){string Ɍ=ȋ[ʺ];if(string.IsNullOrEmpty(Ɍ))continue
;if(!string.IsNullOrEmpty(i)&&Ɍ.IndexOf(i,Ƙ.ƙ)<0)continue;ƥ=true;if(Ȥ>y+0.5f)Ȥ+=ʱ;int ɧ=0;while(ɧ<=Ɍ.Length){int ɨ=Ɍ.
IndexOf('\n',ɧ);string Ū=ɨ<0?Ɍ.Substring(ɧ):Ɍ.Substring(ɧ,ɨ-ɧ);if(Ū.Length==0){if(Ȥ+ʱ>z&&Ȥ<ª)ǐ(" ",Ǣ,Ȥ,ƅ,Ǒ,Ǖ,TextAlignment.LEFT
);Ȥ+=ʱ;}else{if(Ȥ+ʱ>z&&Ȥ<ª){int Ʒ=Ǔ(Ū,Ǣ,Ȥ,ʱ,ƅ,ǔ,Ǖ,TextAlignment.LEFT,Ɖ,Ƌ,true);Ȥ+=Ʒ*ʱ;}else{int Ʒ=Ɵ(Ū,Ɖ,Ƌ);Ȥ+=Ʒ*ʱ;}}if(ɨ<
0)break;ɧ=ɨ+1;}}if(!ƥ&&Ȥ+ʱ>z&&Ȥ<ª)ǐ("(no matching status)",Ǣ,Ȥ,ƅ,Ǒ,Ǖ,TextAlignment.LEFT);}internal static void Ź(string ŷ
,l Ÿ,Dictionary<string,float>ʽ,Dictionary<string,float>ʾ,List<int>ʿ,List<string>ˀ){if(Ÿ.Ř==null||Ÿ.ř==null||Ÿ.Ś==null){ʽ.
Clear();ʾ.Clear();ʿ.Clear();ˀ.Clear();return;}bool ˁ=string.IsNullOrEmpty(ŷ);bool ˆ=string.Equals(ŷ,"OresIngots",Ƙ.ƙ);bool ˇ=
string.Equals(ŷ,"Components",Ƙ.ƙ);ʽ.Clear();ʾ.Clear();ʿ.Clear();ˀ.Clear();int ˈ=Ÿ.Ř.Length;for(int ã=0;ã<ˈ;ã++){if(Ÿ.ř==null||
Ÿ.Ś==null||Ÿ.ř[ã]<=0.001f)continue;string ˉ=Ÿ.Ś[ã]??"";string ˊ=Ÿ.Ř[ã]??"";if(ˉ=="Ore"){float ˋ;ʽ[ˊ]=ʽ.TryGetValue(ˊ,out
ˋ)?ˋ+Ÿ.ř[ã]:Ÿ.ř[ã];}else if(ˉ=="Ingot"){float ˋ;ʾ[ˊ]=ʾ.TryGetValue(ˊ,out ˋ)?ˋ+Ÿ.ř[ã]:Ÿ.ř[ã];}else{ʿ.Add(ã);}}if(!ˇ){if(ˁ
||ˆ){foreach(var Ā in ʽ.Keys)ˀ.Add(Ā);foreach(var Ā in ʾ.Keys){if(!ʽ.ContainsKey(Ā))ˀ.Add(Ā);}}else{foreach(var Ā in ʽ.
Keys){if(string.Equals(Ā,ŷ,Ƙ.ƙ))ˀ.Add(Ā);}foreach(var Ā in ʾ.Keys){if(ʽ.ContainsKey(Ā))continue;if(string.Equals(Ā,ŷ,Ƙ.ƙ))ˀ.
Add(Ā);}}ˀ.Sort(StringComparer.OrdinalIgnoreCase);}ʿ.Sort((Ȧ,Ɍ)=>string.Compare(Ÿ.Ř[Ȧ]??"",Ÿ.Ř[Ɍ]??"",Ƙ.ƙ));if(ˆ)ʿ.Clear();
else if(!ˁ&&!ˇ){for(int ˌ=ʿ.Count-1;ˌ>=0;ˌ--){int ơ=ʿ[ˌ];string ˍ=Ÿ.Ř[ơ]??"";if(!string.Equals(ˍ,ŷ,Ƙ.ƙ))ʿ.RemoveAt(ˌ);}}}
struct ˑ{public string ˎ,ˏ;public Func<A,º>ː;}private static readonly Dictionary<string,Ǵ>ˡ=new Dictionary<string,Ǵ>(
StringComparer.OrdinalIgnoreCase){{"HEAD",Ǵ.Ǭ},{"INV",Ǵ.ǭ},{"REF",Ǵ.Ǯ},{"PWR",Ǵ.ǯ},{"ICE",Ǵ.ǰ},{"WARN",Ǵ.Ǳ},{"STATUS",Ǵ.ǲ},{ȇ.ˠ,Ǵ.ǳ},}
;private static readonly Dictionary<Ǵ,ˑ>ͱ=new Dictionary<Ǵ,ˑ>{{Ǵ.ǭ,new ˑ{ˎ="INV",ˏ="INVENTORY",ː=ˢ}},{Ǵ.Ǯ,new ˑ{ˎ="REF",ˏ
="REFINERY STATUS",ː=ˣ}},{Ǵ.ǯ,new ˑ{ˎ="PWR",ˏ="POWER GRID STATUS",ː=ˤ}},{Ǵ.ǰ,new ˑ{ˎ="ICE",ˏ="ICE STATUS",ː=ˬ}},{Ǵ.Ǳ,new
ˑ{ˎ="WARN",ˏ="WARNING STATUS",ː=ˮ}},{Ǵ.ǲ,new ˑ{ˎ="STATUS",ˏ="SYSTEM STATUS",ː=Ͱ}},};private static º ˢ(A f){return new Ŵ(
f);}private static º ˣ(A f){return new Ͳ(f);}private static º ˤ(A f){return new ͳ(f);}private static º ˬ(A f){return new
ʹ(f);}private static º ˮ(A f){return new Ͷ(f);}private static º Ͱ(A f){return new ͷ(f);}private static Ǵ ɮ(string ͺ){if(
string.IsNullOrEmpty(ͺ))return Ǵ.ǫ;Ǵ Ȣ;return ˡ.TryGetValue(ͺ.Trim(),out Ȣ)?Ȣ:Ǵ.ǫ;}public void Ⱦ(){if(ȝ==null)ȝ=new Dictionary
<string,º>(StringComparer.OrdinalIgnoreCase);else ȝ.Clear();foreach(var ͻ in ͱ){var ͼ=ͻ.Value;if(string.IsNullOrEmpty(ͼ.ˎ
)||ͼ.ː==null)continue;ȝ[ͼ.ˎ]=ͼ.ː(this);}}private static string ʃ(ref Ǹ ʂ){if(ʂ.ǵ==Ǵ.ǫ)return ʂ.Ƿ;ˑ ͼ;return ͱ.TryGetValue
(ʂ.ǵ,out ͼ)?ͼ.ˎ:null;}private static string ʖ(Ǵ Ȣ,string ͽ){if(Ȣ==Ǵ.ǫ)return ͽ!=null?ͽ:"";ˑ ͼ;return ͱ.TryGetValue(Ȣ,out
ͼ)?ͼ.ˏ:"";}sealed class ͳ:º{private readonly A ų;public ͳ(A f){ų=f;}private static int Ί(string i,r s){if(s==null)return
0;if(string.IsNullOrEmpty(i))return 3;int ť=0;string Ά="Batteries x"+s.Ŕ;string Έ="Reactors x"+s.ŕ;string Ή="Engines x"+s
.Ŗ;if(Ά.IndexOf(i,Ƙ.ƙ)>=0)ť++;if(Έ.IndexOf(i,Ƙ.ƙ)>=0)ť++;if(Ή.IndexOf(i,Ƙ.ƙ)>=0)ť++;return ť;}public float v(A f,
VRageMath.Vector2 g,VRageMath.RectangleF h,string i,j k,l m,n o,p q,r s,t u){if(s==null)return g.Y*0.04f;int ŭ=Ί(i,s);return g.Y*
0.035f+ŭ*(g.Y*0.11f)+g.Y*0.02f;}public void µ(A f,MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,string i,
float y,float z,float ª,j k,l m,n o,p q,r s,t u){if(s==null)return;int ŭ=Ί(i,s);float Ȩ=g.Y*0.035f+ŭ*(g.Y*0.11f)+g.Y*0.02f;if
(y+Ȩ<=z||y>=ª)return;if(ŭ==0)return;float Ƨ=x.Width;float ƨ=x.X;float Ȫ=ƨ+Ƨ*0.5f;var Ό=new VRageMath.Vector2(Ƨ,g.Y);float
Ύ=s.Ŏ>1e-6f?s.Ŏ:1f;float Ώ=ǃ.Ǆ(s.Ō/Ύ,0f,1f);float ΐ=s.ŏ>1e-6f?s.ŏ:1f;float Α=ǃ.Ǆ(s.ő/ΐ,0f,1f);float Β=s.Ő>1e-6f?s.Ő:1f;
float Γ=ǃ.Ǆ(s.Œ/Β,0f,1f);string Δ="Batteries x"+s.Ŕ;string Ε="Reactors x"+s.ŕ;string Ζ="Engines x"+s.Ŗ;var ǉ=new string[ŭ];
var Ǌ=new float[ŭ];var ǋ=new string[ŭ];int ơ=0;if(string.IsNullOrEmpty(i)||Δ.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=Δ;Ǌ[ơ]=Ώ;ǋ[ơ]="OUT:"+s
.Ō.ToString("0.0")+" IN:"+s.ŋ.ToString("0.0");ơ++;}if(string.IsNullOrEmpty(i)||Ε.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=Ε;Ǌ[ơ]=Α;ǋ[ơ]=
"OUT:"+s.ő.ToString("0.0");ơ++;}if(string.IsNullOrEmpty(i)||Ζ.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=Ζ;Ǌ[ơ]=Γ;ǋ[ơ]="OUT:"+s.Œ.ToString("0.0")
;ơ++;}ų.ǎ(y,Ό,new VRageMath.Vector2(Ȫ,0f),ǉ,Ǌ,ǋ,new VRageMath.Color(255,0,0,200),true);}}sealed class ʹ:º{private
readonly A ų;public ʹ(A f){ų=f;}private static int Λ(string i,p q){if(q==null)return 0;if(string.IsNullOrEmpty(i))return 4;int ť
=0;string Η="Total";string Θ="Generators x"+q.Ņ;string Ι="Irrigation x"+q.ņ;string Κ="Cargo";if(Η.IndexOf(i,Ƙ.ƙ)>=0)ť++;
if(Θ.IndexOf(i,Ƙ.ƙ)>=0)ť++;if(Ι.IndexOf(i,Ƙ.ƙ)>=0)ť++;if(Κ.IndexOf(i,Ƙ.ƙ)>=0)ť++;return ť;}public float v(A f,VRageMath.
Vector2 g,VRageMath.RectangleF h,string i,j k,l m,n o,p q,r s,t u){if(q==null)return g.Y*0.04f;int ŭ=Λ(i,q);return g.Y*0.035f+ŭ
*(g.Y*0.11f)+g.Y*0.02f;}public void µ(A f,MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,string i,float y
,float z,float ª,j k,l m,n o,p q,r s,t u){if(q==null)return;int ŭ=Λ(i,q);float Ȩ=g.Y*0.035f+ŭ*(g.Y*0.11f)+g.Y*0.02f;if(y+
Ȩ<=z||y>=ª)return;if(ŭ==0)return;float Ƨ=x.Width;float ƨ=x.X;float Ȫ=ƨ+Ƨ*0.5f;var Ό=new VRageMath.Vector2(Ƨ,g.Y);string Μ
="Total";string Ν="Generators x"+q.Ņ;string Ξ="Irrigation x"+q.ņ;string Ο="Cargo";var ǉ=new string[ŭ];var Ǌ=new float[ŭ];
var ǋ=new string[ŭ];int ơ=0;if(string.IsNullOrEmpty(i)||Μ.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=Μ;Ǌ[ơ]=q.ŀ;ǋ[ơ]=ƛ.Ɯ(q.ļ);ơ++;}if(string.
IsNullOrEmpty(i)||Ν.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=Ν;Ǌ[ơ]=q.Ł;ǋ[ơ]=ƛ.Ɯ(q.Ľ);ơ++;}if(string.IsNullOrEmpty(i)||Ξ.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=Ξ;Ǌ[ơ
]=q.ł;ǋ[ơ]=ƛ.Ɯ(q.ľ);ơ++;}if(string.IsNullOrEmpty(i)||Ο.IndexOf(i,Ƙ.ƙ)>=0){ǉ[ơ]=Ο;Ǌ[ơ]=q.Ń;ǋ[ơ]=ƛ.Ɯ(q.Ŀ);ơ++;}ų.ǎ(y,Ό,new
VRageMath.Vector2(Ȫ,0f),ǉ,Ǌ,ǋ,new VRageMath.Color(165,220,255,200),true);}}sealed class Ͳ:º{private readonly A ų;public Ͳ(A f){ų=
f;}public float v(A f,VRageMath.Vector2 g,VRageMath.RectangleF h,string i,j k,l m,n o,p q,r s,t u){if(o==null||o.İ==null)
return g.Y*0.04f;float Π=g.Y*0.072f;if(string.IsNullOrEmpty(i)){int Ρ=o.İ.Length;int Σ=Ρ>0?(Ρ+1)/2:1;return g.Y*0.180f+Σ*Π+g.Y
*0.02f;}if(string.Equals(i,"Priority",Ƙ.ƙ))return g.Y*0.180f;int Τ=0;int ť=o.İ.Length;for(int ã=0;ã<ť;ã++){string ˍ=o.İ[ã
]??"";if(ˍ.IndexOf(i,Ƙ.ƙ)>=0)Τ++;}int Υ=Τ>0?(Τ+1)/2:0;return g.Y*0.08f+Υ*Π+g.Y*0.02f;}public void µ(A f,MySpriteDrawFrame
w,VRageMath.Vector2 g,VRageMath.RectangleF x,string i,float y,float z,float ª,j k,l m,n Ċ,p q,r s,t u){if(Ċ==null||Ċ.İ==
null)return;float Π=g.Y*0.072f;float Φ;if(string.IsNullOrEmpty(i)){int Χ=Ċ.İ.Length;int Σ=Χ>0?(Χ+1)/2:1;Φ=g.Y*0.180f+Σ*Π+g.Y
*0.02f;}else if(string.Equals(i,"Priority",Ƙ.ƙ))Φ=g.Y*0.180f;else{int Ψ=0;for(int ˌ=0;ˌ<Ċ.İ.Length;ˌ++){if((Ċ.İ[ˌ]??"").
IndexOf(i,Ƙ.ƙ)>=0)Ψ++;}int Υ=Ψ>0?(Ψ+1)/2:0;Φ=g.Y*0.08f+Υ*Π+g.Y*0.02f;}if(y+Φ<=z||y>=ª)return;float Ƨ=x.Width;float ƨ=x.X;float
Ȫ=ƨ+Ƨ*0.5f;float Ω=Ƨ*0.5f;const float Ϊ=0.52f;const float Ϋ=0.58f;float ά=g.Y*0.038f;if(string.Equals(i,"Priority",Ƙ.ƙ)){
string έ=Ċ.ĺ;string ή=Ċ.Ļ;if(string.IsNullOrEmpty(έ)){έ="1. Fe  2. Co  3. Ni";ή=null;}ų.ǐ(έ,Ȫ,y+g.Y*0.025f,0.72f,A.ǔ,A.ǒ,
TextAlignment.CENTER);if(!string.IsNullOrEmpty(ή))ų.ǐ(ή,Ȫ,y+g.Y*0.075f,0.72f,A.ǔ,A.ǒ,TextAlignment.CENTER);return;}if(string.
IsNullOrEmpty(i)){string έ=Ċ.ĺ;string ή=Ċ.Ļ;if(string.IsNullOrEmpty(έ)){έ="1. Fe  2. Co  3. Ni";ή=null;}ų.ǐ(έ,Ȫ,y+g.Y*0.025f,0.72f,A.
ǔ,A.ǒ,TextAlignment.CENTER);if(!string.IsNullOrEmpty(ή))ų.ǐ(ή,Ȫ,y+g.Y*0.075f,0.72f,A.ǔ,A.ǒ,TextAlignment.CENTER);}float ί
=string.IsNullOrEmpty(i)?y+g.Y*0.180f:y+g.Y*0.08f;int Ρ=Ċ.İ.Length;int ΰ=0;for(int ã=0;ã<Ρ;ã++){if(!string.IsNullOrEmpty(
i)){string α=Ċ.İ[ã]??"";if(α.IndexOf(i,Ƙ.ƙ)<0)continue;}int β=ΰ%2;int γ=ΰ/2;ΰ++;float δ=ƨ+β*Ω;float ε=ί+γ*Π;float ζ=ε-g.Y
*0.018f;float η=δ+Ω*0.065f;string θ=Ċ.İ[ã]??"Unknown Refinery";bool ķ=(Ċ.ķ!=null&&ã<Ċ.ķ.Length)?Ċ.ķ[ã]:false;bool Ĺ=(Ċ.Ĺ
!=null&&ã<Ċ.Ĺ.Length)?Ċ.Ĺ[ã]:false;string ι=(Ċ.Ĳ!=null&&ã<Ċ.Ĳ.Length)?Ċ.Ĳ[ã]:"";var κ=A.Ǒ;if(ķ)κ=A.Ǩ;else if(Ĺ)κ=A.ǧ;
string μ=Ĺ&&!string.IsNullOrEmpty(ι)?ƛ.λ(ι):"-";ų.ǐ(μ,δ+Ω*0.24f,ζ,Ϊ,new VRageMath.Color(220,220,220,255),A.Ǖ,TextAlignment.
CENTER);ų.ǐ(θ,δ+Ω*0.36f,ζ,Ϋ,A.ǔ,A.ǒ,TextAlignment.LEFT);ų.ȩ("Circle",η,ε,ά,ά,κ);}}}sealed class Ͷ:º{private readonly A ų;
public Ͷ(A f){ų=f;}public float v(A f,VRageMath.Vector2 g,VRageMath.RectangleF h,string i,j k,l m,n o,p q,r s,t u){if(u==null
||u.Ţ)return g.Y*0.22f;int ʥ=0;if(u.ŗ)ʥ++;if(u.ś)ʥ++;if(u.Ň)ʥ++;if(u.ŝ)ʥ++;if(u.Ş)ʥ++;if(u.Ŝ)ʥ++;if(ʥ==0)ʥ=1;return ʥ*(g.Y
*0.065f)+g.Y*0.02f;}public void µ(A f,MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,string i,float y,
float z,float ª,j k,l m,n o,p q,r s,t u){int ʥ=0;if(u!=null&&!u.Ţ){if(u.ŗ)ʥ++;if(u.ś)ʥ++;if(u.Ň)ʥ++;if(u.ŝ)ʥ++;if(u.Ş)ʥ++;if(
u.Ŝ)ʥ++;if(ʥ==0)ʥ=1;}float Φ=u==null||u.Ţ?g.Y*0.22f:ʥ*(g.Y*0.065f)+g.Y*0.02f;if(y+Φ<=z||y>=ª)return;if(u==null)return;
float Ƨ=x.Width;float ƨ=x.X;float Ȫ=ƨ+Ƨ*0.5f;float ʝ=Math.Min(1f,Ƨ/350f);if(u.Ţ){ų.ǐ("ALL SYSTEMS NOMINAL",Ȫ,y+g.Y*0.13f,1.0f
*ʝ,A.Ǩ,A.ǒ,TextAlignment.CENTER);return;}ų.Ȋ.Clear();if(u.ŗ)ų.Ȋ.Add("LOW POWER");if(u.ś)ų.Ȋ.Add("CARGO FULL");if(u.Ň)ų.Ȋ.
Add("LOW ICE");if(u.ŝ)ų.Ȋ.Add("REFINERY STALLED");if(u.Ş)ų.Ȋ.Add("ASSEMBLER STALLED");if(u.Ŝ)ų.Ȋ.Add("NO REFINERIES");float
Ȥ=y+g.Y*0.02f;float ν=g.Y*0.065f;for(int ã=0;ã<ų.Ȋ.Count;ã++){string ȧ=ų.Ȋ[ã];ų.ǐ(ȧ,Ȫ,Ȥ,0.92f*ʝ,A.ǧ,A.ǒ,TextAlignment.
CENTER);Ȥ+=ν;}}}sealed class ͷ:º{private readonly A ų;private const float ξ=0.52f;public ͷ(A f){ų=f;}public float v(A f,
VRageMath.Vector2 g,VRageMath.RectangleF h,string i,j k,l m,n o,p q,r s,t u){return ų.ʻ(g,h,i??"",ξ);}public void µ(A f,
MySpriteDrawFrame w,VRageMath.Vector2 g,VRageMath.RectangleF x,string i,float y,float z,float ª,j k,l m,n o,p q,r s,t u){float Φ=ų.ʻ(g,x,
i??"",ξ);if(y+Φ<=z||y>=ª)return;ų.ʼ(g,x,i??"",y,z,ª,ξ);}}}public sealed class ȇ{public const string ˠ="COL";float ο,π;
float ρ,ς,σ,τ;int υ;public VRageMath.RectangleF ʅ{get;private set;}public void ɸ(float φ,float χ){ο=φ;ρ=χ;π=0f;ς=0f;σ=0f;τ=0f
;υ=0;ʅ=new VRageMath.RectangleF(0f,0f,ο,ρ);}public float ʁ{get{if(υ==0)return π;return Math.Max(π,ς+Math.Max(σ,τ));}}
public float ʑ{get{if(υ==0)return π;if(υ==1)return ς+σ;return ς+τ;}}public float ʕ{get{return ʅ.X+ʅ.Width*0.5f;}}public void ɺ
(string ψ){string Ȧ=ψ==null?"":ψ.Trim();if(Ȧ.Length==0){ω();return;}if(string.Equals(Ȧ,"FULL",Ƙ.ƙ)){ϊ();return;}if(string
.Equals(Ȧ,"LEFT",Ƙ.ƙ)){ϋ();return;}if(string.Equals(Ȧ,"RIGHT",Ƙ.ƙ)){ό();return;}}void ω(){if(υ==0||υ==2){ύ();ς=π;σ=0f;τ=
0f;υ=1;ώ(1);return;}if(υ==1){υ=2;ώ(2);}}void ϋ(){if(υ==1||υ==2)ύ();ς=π;σ=0f;τ=0f;υ=1;ώ(1);}void ό(){if(υ==0){ς=π;σ=0f;τ=0f
;}else if(υ==1){}else if(υ==2){ύ();ς=π;σ=0f;τ=0f;}υ=2;ώ(2);}public void ϊ(){ύ();υ=0;ʅ=new VRageMath.RectangleF(0f,0f,ο,ρ)
;}public void ɿ(float Ϗ){if(υ==0){π+=Ϗ;return;}if(υ==1)σ+=Ϗ;else τ+=Ϗ;}public void ʀ(){ύ();}void ύ(){if(υ==0)return;float
ϐ=ς+Math.Max(σ,τ);if(ϐ>π)π=ϐ;υ=0;σ=0f;τ=0f;ʅ=new VRageMath.RectangleF(0f,0f,ο,ρ);}void ώ(int β){float ϑ=ο*0.5f;if(β==1)ʅ=
new VRageMath.RectangleF(0f,0f,ϑ,ρ);else ʅ=new VRageMath.RectangleF(ϑ,0f,ϑ,ρ);}}public static class Ĉ{public static ċ ĉ<ċ>(
string ù){try{if(ù==null)return default(ċ);return Ų.ĉ<ċ>(ù);}catch{}return default(ċ);}}public class C{}public class p{public
float ļ,Ľ,ľ,Ŀ,ŀ,Ł,ł,Ń;public int Ņ,ņ;public bool Ň;}public class l{public string[]Ř,Ś;public float[]ř;}public class j{public
float ĕ,Ė,ė,Ę,ę,Ě,ě,Ĝ,ĝ,Ğ,ğ,Ġ,ġ,Ģ,ģ,Ĥ,ĥ,Ħ,ħ,Ĩ,ĩ,Ī,ī,Ĭ,ĭ,Į,į;}public class r{public float ŉ,Ŋ,ŋ,Ō,ō,Ŏ,ő,Œ,ŏ,Ő;public int Ŕ,ŕ,Ŗ
;public bool ŗ;}public class n{public string[]İ,Ĳ,ĵ;public float[]ĳ,Ķ;public bool[]ķ,Ĺ;public string ĺ,Ļ;}public class t{
public bool Ň,ŗ,ś,Ŝ,ŝ,Ş,Ţ;public int Š;public string š;}public static class Ô{public const string Û="SYS_STATUS",Ü=
"PB1_WARNINGS",Õ="PB1ToPB2_InventorySummary",Ö="PB1ToPB2_RefineryStatus",Ø="PB1ToPB2_IceStatus",Ù="PB1ToPB2_PowerStatus",Ú=
"PB1ToPB2_InventoryDynamic";}public static class ô{private const uint ϒ=2166136261u,ϓ=16777619u;public const long ö=90L*TimeSpan.TicksPerSecond;
public static bool ć(string ϔ,string ϕ,Dictionary<string,long>ϖ,Dictionary<string,long>ϗ,long Ă,long Ϙ,out string T,out string
ϙ){T=null;ϙ=null;if(ϔ==null||ϖ==null){return false;}string[]Ŧ=ϔ.Split(new[]{'|'},4);if(Ŧ.Length!=4){return false;}string
Ϛ=Ŧ[0];string ϛ=Ŧ[1];string Ϝ=Ŧ[2];string ϝ=Ŧ[3];if(Ϛ==null||ϛ==null||Ϝ==null||ϝ==null){return false;}string Ϟ;if(Ϝ.
Length==0){Ϟ="";}else{byte[]Ý;try{Ý=Convert.FromBase64String(Ϝ);}catch{return false;}Ϟ=Encoding.UTF8.GetString(Ý);}long ϟ;if(!
long.TryParse(ϛ,out ϟ)){return false;}long Ϡ=0;bool ϡ=ϗ!=null&&Ϙ>0;long ˋ;if(ϖ.TryGetValue(Ϛ,out ˋ)){if(ϡ){long Ϣ;if(ϗ.
TryGetValue(Ϛ,out Ϣ)){if(Ă-Ϣ>Ϙ){ϖ.Remove(Ϛ);ϗ.Remove(Ϛ);Ϡ=0;}else{Ϡ=ˋ;}}else{ϖ.Remove(Ϛ);Ϡ=0;}}else{Ϡ=ˋ;}}if(ϟ<=Ϡ){return false;}
string ϣ=ϕ??"";uint Ȩ=ϒ;Ȩ=Ϥ(Ȩ,Ϛ);Ȩ=Ϥ(Ȩ,ϛ);Ȩ=Ϥ(Ȩ,Ϟ);Ȩ=Ϥ(Ȩ,ϣ);string ϥ=Ȩ.ToString("X8");if(!string.Equals(ϝ,ϥ,StringComparison.
Ordinal)){return false;}ϖ[Ϛ]=ϟ;if(ϡ){ϗ[Ϛ]=Ă;}T=Ϛ;ϙ=Ϟ;return true;}public static void õ(Dictionary<string,long>ϖ,Dictionary<
string,long>ϗ,long Ă,long Ϙ,List<string>Ϧ){if(ϖ==null||ϗ==null||Ϧ==null){return;}if(Ϙ<=0){return;}Ϧ.Clear();foreach(
KeyValuePair<string,long>ÿ in ϗ){if(Ă-ÿ.Value>Ϙ){Ϧ.Add(ÿ.Key);}}for(int ã=0;ã<Ϧ.Count;ã++){string Ā=Ϧ[ã];ϖ.Remove(Ā);ϗ.Remove(Ā);}Ϧ.
Clear();foreach(string Ā in ϖ.Keys){if(!ϗ.ContainsKey(Ā)){Ϧ.Add(Ā);}}for(int ã=0;ã<Ϧ.Count;ã++){ϖ.Remove(Ϧ[ã]);}}private
static uint Ϥ(uint ϧ,string ţ){if(ţ==null||ţ.Length==0){return ϧ;}for(int ã=0;ã<ţ.Length;ã++){char ű=ţ[ã];ϧ^=(byte)(ű&0xFF);ϧ
*=ϓ;ϧ^=(byte)((ű>>8)&0xFF);ϧ*=ϓ;}return ϧ;}}public static class ɡ{public static bool ɢ(string Ƣ,string ă){if(string.
IsNullOrEmpty(Ƣ)||string.IsNullOrEmpty(ă))return false;return Ƣ.IndexOf(ă,StringComparison.OrdinalIgnoreCase)>=0;}}public static
class ƛ{private static readonly StringBuilder Ϩ=new StringBuilder(48);public static string ǆ(float ϩ){if(float.IsNaN(ϩ))
return"NaN%";if(float.IsInfinity(ϩ))return ϩ>0f?"Infinity%":"-Infinity%";int ē=(int)Math.Round((double)ϩ);Ϩ.Clear();Ϩ.Append(ē
.ToString());Ϩ.Append('%');return Ϩ.ToString();}public static string Ɯ(float ϩ){if(float.IsNaN(ϩ))return"NaN";if(float.
IsInfinity(ϩ))return ϩ>0f?"Infinity":"-Infinity";bool Ϫ=ϩ<0f;double ϫ=Ϫ?-(double)ϩ:(double)ϩ;string è="";double Ϭ=1.0;if(ϫ>=1e9){è
="B";Ϭ=1e9;}else if(ϫ>=1e6){è="M";Ϭ=1e6;}else if(ϫ>=1e3){è="k";Ϭ=1e3;}Ϩ.Clear();if(Ϫ)Ϩ.Append('-');if(è.Length>0){double
ʣ=ϫ/Ϭ;ʣ=Math.Round(ʣ*10.0)/10.0;Ϩ.Append(ʣ.ToString("0.0"));Ϩ.Append(è);}else{float ϭ=Ϫ?-(float)ϫ:(float)ϫ;Ϩ.Append(ϭ.
ToString("0.######"));}return Ϩ.ToString();}public static string λ(string Ϯ){if(string.IsNullOrEmpty(Ϯ)){return"-";}if(string.
Equals(Ϯ,"Iron",Ƙ.ƙ)){return"Fe";}if(string.Equals(Ϯ,"Nickel",Ƙ.ƙ)){return"Ni";}if(string.Equals(Ϯ,"Cobalt",Ƙ.ƙ)){return"Co";}
if(string.Equals(Ϯ,"Silicon",Ƙ.ƙ)){return"Si";}if(string.Equals(Ϯ,"Silver",Ƙ.ƙ)){return"Ag";}if(string.Equals(Ϯ,"Gold",Ƙ.ƙ
)){return"Au";}if(string.Equals(Ϯ,"Magnesium",Ƙ.ƙ)){return"Mg";}if(string.Equals(Ϯ,"Platinum",Ƙ.ƙ)){return"Pt";}if(string
.Equals(Ϯ,"Uranium",Ƙ.ƙ)){return"U";}if(string.Equals(Ϯ,"Stone",Ƙ.ƙ)){return"St";}if(string.Equals(Ϯ,"Ice",Ƙ.ƙ)){return
"Ic";}if(Ϯ.Length<=2){return Ϯ.ToUpperInvariant();}return Ϯ.Substring(0,2).ToUpperInvariant();}}public static class ǃ{public
static float Ǆ(float ϩ,float ϯ,float ǈ){if(ϯ>ǈ){float ϰ=ϯ;ϯ=ǈ;ǈ=ϰ;}if(ϩ<ϯ)return ϯ;if(ϩ>ǈ)return ǈ;return ϩ;}}public static
class Ƙ{public const StringComparison ƙ=StringComparison.OrdinalIgnoreCase;