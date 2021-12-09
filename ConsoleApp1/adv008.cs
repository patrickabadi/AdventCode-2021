using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class adv008
    {
        public void Run()
        {
            

            var entries = ParseEntries(input2);

            Debug.WriteLine($"{entries.Count} entries found");

            //var res = CountUniqueOutputValues(entries);

            var res = DeduceNumbers(entries);

            Debug.WriteLine($"Result {res}");
        }

        public int DeduceNumbers(List<Entry> entries)
        {
            int totals = 0;

            foreach (var entry in entries)
            {
                foreach (var inp in entry.Inputs)
                {

                    inp.DeduceDigit(numberSystem);

                    
                }

                foreach (var opt in entry.Outputs)
                {
                    opt.DeduceDigit(numberSystem);
                }

                //DebugNumbers(entry.Inputs);
                ResetMappings();

                var one = entry.Inputs.First(_ => _.Digit == 1);
                var four = entry.Inputs.First(_ => _.Digit == 4);
                var seven = entry.Inputs.First(_ => _.Digit == 7);
                var eight = entry.Inputs.First(_ => _.Digit == 8);

                var diff = one.Code ^ seven.Code;
                SetMapping(diff, 'a');

                var map = four.Code | mapping_code;
                var nine = entry.Inputs.First(_ => _.NumberSegments == 6 && _.Digit < 0 && IsExclusive(_.Code ^ map));
                diff = four.Code ^ nine.Code ^ mapping_code;
                SetMapping(diff, 'g');
                nine.Digit = 9;

                
                map = seven.Code | mapping_code;
                var three = entry.Inputs.First(_ => _.NumberSegments==5 && _.Digit < 0 && IsExclusive(_.Code ^ map & _.Code));
                diff = three.Code ^ map & three.Code;
                SetMapping(diff, 'd');
                three.Digit = 3;
                //DebugNumbers(entry.Inputs);


                map = nine.Code;
                var six = entry.Inputs.First(_ => _.NumberSegments == 6 && _.Digit < 0 && IsExclusive(_.Code ^ map & _.Code) && (_.Code & one.Code) != one.Code);
                diff = six.Code ^ map & six.Code;
                SetMapping(diff, 'd');
                six.Digit = 6;
                //DebugNumbers(entry.Inputs);


                map = mapping_code;
                var two = entry.Inputs.First(_ => _.NumberSegments == 5 && _.Digit < 0 && IsExclusive(_.Code ^ map & _.Code));
                diff = two.Code ^ map & two.Code;
                SetMapping(diff, 'c');
                two.Digit = 2;
                //DebugNumbers(entry.Inputs);

                map = nine.Code;
                var zero = entry.Inputs.First(_ => _.NumberSegments == 6 && _.Digit < 0 && IsExclusive(_.Code ^ map & _.Code));
                diff = zero.Code ^ map & zero.Code;
                //SetMapping(diff, 'c');
                zero.Digit = 0;
                //DebugNumbers(entry.Inputs);

                var five = entry.Inputs.First(_ => _.Digit < 0);
                five.Digit = 5;

                //DebugNumbers(entry.Inputs);

                string output = string.Empty;
                foreach(var outp in entry.Outputs)
                {
                    var num = entry.Inputs.First(_ => _.Code == outp.Code);
                    output += num.Digit.ToString();
                }
                totals += int.Parse(output);

            }

            return totals;
        }

        public void ResetMappings()
        {
            mapping_code = 0;
            for(int i=0; i<mapping.Length; i++)
            {
                mapping[i] = ' ';
            }
        }

        public void DebugMappings()
        {
            for(int i=0; i<mapping.Length; i++)
            {
               // Debug.WriteLine($"mapping ")
            }
        }

        public void DebugNumbers(List<Number> numbers)
        {
            foreach(var n in numbers)
            {
                string dig = n.Digit < 0 ? " " : n.Digit.ToString();
                Debug.WriteLine($"input {dig}\t\t{n.Segment}\t\t{DebugOutCode(n.Code)}");
            }
        }

        public bool IsExclusive(int code)
        {
            var str = Convert.ToString(code, 2);
            int count = 0;
            for(int i=0; i<str.Length;i++)
            {
                if (str[i] == '1')
                    count++;
            }
            return count == 1;
        }

        public void SetMapping(int diff, char map)
        {
            //Debug.WriteLine($"SetMapping {DebugOutCode(diff)} {GetLetter(diff)} map to {map}");
            mapping_code |= diff;
            mapping[GetLetter(diff) - 'a'] = map;
        }

        public char GetLetter(int code)
        {
            for(int i=0; i<7; i++)
            {
                if (code == 1)
                    return (char)('g' - i);
                code = code >> 1;
            }

            return 'x';
        }

        public string DebugOutCode(int code)
        {
            return Convert.ToString(code, 2);
        }

        public int CountUniqueOutputValues(List<Entry> entries)
        {
            int count = 0;

            foreach(var entry in entries)
            {
                foreach(var output in entry.Outputs)
                {
                    if (output.DeduceDigit(numberSystem))
                        count++;
                }
            }

            return count;
        }

        public List<Entry> ParseEntries(string inp)
        {
            List<Entry> entries = new List<Entry>();

            var lines = inp.Split('\n');

            foreach(var line in lines)
            {
                entries.Add(new Entry(line.Trim()));
            }

            return entries;
        }

        public class Entry
        {
            public List<Number> Inputs;
            public List<Number> Outputs;

            public Entry(string line)
            {
                Inputs = new List<Number>();
                Outputs = new List<Number>();

                var sep = line.Split('|');

                var inps = sep[0].Split(' ');
                foreach(var inp in inps)
                {
                    if (string.IsNullOrWhiteSpace(inp))
                        continue;
                    Inputs.Add(new Number(inp));
                }

                var otps = sep[1].Split(' ');
                foreach (var otp in otps)
                {
                    if (string.IsNullOrWhiteSpace(otp))
                        continue;
                    Outputs.Add(new Number(otp));
                }
            }
        }

        public class Number
        {
            public int Digit;
            public int Code;
            public int NumberSegments;
            public string Segment;

            public Number(string segment, int digit = -1)
            {
                Digit = digit;
                Code = 0;
                NumberSegments = segment.Length;

                var codeArr = "0000000".ToCharArray();
                for (int i=0; i<segment.Length; i++)
                {
                    var letter = segment[i];

                    codeArr[(int)(letter - 'a')] = '1'; 
                }

                Segment = String.Concat(segment.OrderBy(c => c));

                var codeString = new string(codeArr);
                Code = Convert.ToInt32(codeString, 2);
            }

            public bool DeduceDigit(List<Number> numberSystem)
            {
                if (NumberSegments == numberSystem[1].NumberSegments)
                    Digit = 1;
                else if (NumberSegments == numberSystem[4].NumberSegments)
                    Digit = 4;
                else if (NumberSegments == numberSystem[7].NumberSegments)
                    Digit = 7;
                else if (NumberSegments == numberSystem[8].NumberSegments)
                    Digit = 8;
                else
                    return false;

                return true;
            }
        }

        char[] mapping = new char[7];
        int mapping_code = 0;

        List<Number> numberSystem = new List<Number>
            {
                new Number("abcefg", 0), // abc efg
                new Number("cf", 1),     //   c  f
                new Number("acdeg", 2),  // a cde g
                new Number("acdfg", 3),  // a cd  g
                new Number("bcdf", 4),   // ab d  g
                new Number("abdfg", 5),  // ab d fg
                new Number("abdefg", 6), // ab defg
                new Number("acf", 7),    // a c  f
                new Number("abcdefg", 8),// abcdefg
                new Number("abcdfg", 9)  // abcd fg
            };

        public string input0 = "acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab | cdfeb fcadb cdfeb cdbaf";
        public string input00 = "aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea";

        public string input = @"be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe
edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc
fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg
fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb
aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea
fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb
dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe
bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef
egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb
gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce";

        public string input2 = @"dcbgefa cebd bfega eadbf db cdfaeb dba bfcgda egadcf aedcf | egadcfb eafcd db debc
dfea ea abe dfgaeb fdcgba badeg dcfbeag dbgaf bcged fcebag | gfadb dfagb fdgabe eagdbf
cd gfcde cdbfage cbdfae edc fedgac gadc feagd febdga gbfec | adbfeg dbfcae facbde gcfde
ecafb abgfc deacbgf fe fbe fgec edgafb cgbafd ebfgca ebcad | cfbae fbcga dfabceg ceabf
eagfcd abfdecg fe fdae egf cfgda gcedb dcgabf bcegaf cegdf | gcfda feg efg cadgbf
gfcdab efadc cadbgfe beca badfce ecfbd edcagf gbdef cfb bc | gdecbfa cbea gedbf aceb
cgafb abgcde debfca ecafgd ecdag bcd dacbgfe gebd db cabgd | cfdaeg dcfeag cbgda agfcb
cegba dg gbd feabgd fdecgb gbced fdbcae dfcg fbced gdceafb | gfecdab dbg dg fdceabg
cebgdaf gbfaec cebg fbcag bac efagb cgdaf fcbade fgadeb cb | abcgf fdeabg dbafge cadgf
aefdg ga becfagd dag gfdbe bgeacd dgaefb caefd cfebdg bafg | gda afegdb afdce efacd
dfacgb afegc cbfgae dceg gcafbde dfabe gadef gd cfegda adg | bcafeg efadb efbda cgefa
gbfae cbgf ceg dgefba cafde bcagde dbfaegc gefca egcfba cg | abefg agfbe cafgeb fgeadbc
cabef ecdfbg gceabf facg dbage ecfabd eagbfcd gc gbc eabcg | bfedcg cfbgde acebg bcfadeg
bcedagf abecf cdaebf gacb dgfec gaf gebafc bgfdea ag cefga | egfac dfcbea agf deacbf
febagcd bfgcae fecab abcfde dgbec da fadebg ade adbce dfca | gbefdca aedfgb adebgf cbdge
acb fgadc ebaf efdbgc ab dbgaec caegbfd fdcbe abcedf cfdba | ecdfb bfdcea ebcdga cedfb
cbaeg befda bdagfe gd dga dgbae deacgf fbgdeac dfbg decbfa | gecafd gad ceagb debaf
egd de bfadeg gcfbe gdbcafe bgfeca cedgfb cdagf edgcf bedc | cefgd ecgafb edg ged
aebfgc bgf bcag fceabd gcdef becdafg befac gb fcegb badegf | ebfdga cgba gabecf cfedba
abgdfe gafdc fdb egbdfca cgbaef edba egfba cgefdb bd agdfb | dfb fgdca cgafd fgdba
agfeb dgafb agfecb gfcaed abegc febc ef fea ebfcdga bedagc | ebcadg eaf bafge gcaeb
ba febcag gfbec fcab bgfecd dgeaf eacbdg fagedcb efgab abg | egfdbc geabcd cegbf ecfbgd
fage egacd cdagb dcefag gcedfb efgcd cedgfba fdcbea dae ea | ae aed gfedbc faeg
gefac cbe cb fgacdeb bceadg agfebc fedgca abfc egcfb ebdfg | egbfc fbdeg aefdcg gcbfe
agbfce edbga agfde cfage afcged bgfcde bcagedf cdaf fdg df | bdcegaf dgefa eadgb fcgdae
acebg ce bcadg gcedfb ebc bfeag degfba acfe bacdgef baefgc | aebdfg abgfe abecgf cgabd
bgafd dgbae dbcfae acfegd bcfg cgafd fdbgca fb dcbgafe bfa | afdbg baf bfgc bf
daebcf fcdgbe egfab gaedb da agcdbe dagc adebfcg bda cgbde | gfabe gdecab eabdg cdgbfae
decbga bdac da cgdfe ebacg agbfce cagde fecbagd gda dfegba | da fcedg fcdge dga
dfecba fbc gceab ebfg acdegb bf afdebgc befagc cgdfa bgfac | baecgd gacfbe bf cfagb
bgefca agcbf dfgceab bcedfa ebafg fdgae feb dcgabf gebc be | agbfe be becfga gcbafe
cbgaed gb dgb defcg gdfeb ecafgd dbgfce bcfg facegbd deabf | gdcef adgbec egdfb acgefd
bcead bdecag adbfec gda gabe febgcda ag caedg gabdfc efcdg | ebag ageb cedafb edbca
bg cdbga ebcg adgfeb dagbefc dbg caedg cgfdae acfdb geadbc | cabdf gbce bacdeg dabegf
fecdgab bd gfcade agedf cafbg cagdbe adbgf gbd efbgad efbd | egafdc gfdea bfaedcg gabedc
cfdgbe cgaeb edab gacde ebg be edcagbf edcgaf abfcg adcgbe | gafbc gcbaf adgbec be
geb bdfg abced fdbaeg gdafe gfdecba bg gcedaf bdage ebfgca | cgefda egdbaf gdcfae eagdb
cafbge cfdgb abfce aedfbg cade de beafcgd cdbef dbcfae def | dfbaec cfdbe gcaefb dgbfc
bafce bcage ebcagfd fc decbfa bdcf fdaeb acdegf efc egbadf | aefbd abegc eabdf bfdea
gc dfeacg ebfacdg ebcdfa bdfac gbcda fbcg dafbcg cag bdega | dcabfe cbfg fabdc gcfb
fdaeb agbfe ecadbg efdacgb dbf debac cdaf dgcfbe dacbfe fd | abdfe gfeab fdac dfb
cbfdg adgcb gfebc gedabc bdf df agcfbd dgaf gfdebac ecadbf | fcgbd bgdca adbfgc gfad
bgf aefbg fgdcab ecbfadg eagbc bgedac fbec fgabec fb egadf | bfec afged ebfc gefad
cgafdbe fedgab gafde adbg gebcfd efcag bcdfae fdg fdeba gd | fgabedc dg cegaf cfbaed
bfecagd gcfdb ce fgabed gbeafc ecdbf bce deca acfdeb beadf | bce agfecb gedbaf dfbec
dfb abcfge fdbage acefd cgbd db gefbc bcdfe acdgfbe gbdcfe | fcdbe fdecgb fgcbe fceda
efdac gacbed cafeb fcgd fdaeg cd gebdfa fdbagec ecd dacfeg | edafcg deacgf eadcgb dgfc
degabc egc cegfa gdefa gafbc ce cfed cagedfb aefgdc eafdgb | gdfeab agfcb gebfda fadge
aegbdc dbgfec begac ef fdgca efc gacebf eafgc bafe edcfbga | acfeg bfgeca cef befa
cfegdba bdage bgc faceb cbafge bdefca gdfabc caebg gc gefc | cfbead cg cg eafbc
db bdaegf abd debgfac feacb edafg dfbcga dafeb agdefc debg | eafcb bafed dafgbe edfba
dbegac efdcg ecdgfa geafc fabgdec efcab gfad ag ega dcebfg | defgacb efgcbd dagf fgad
dafbe bdfgae cd cadfe ecfdab bcdefag gface dcfb acd cbegad | efabd dcbf decfbga dbgeaf
ca gfbdc eadcfgb cfgade efac afdeg dcgaeb dgfabe gca gdfac | dacbeg gcefda cfdbg geabfd
fbecag fdaegc gfade edcafgb ecf cfabd ecdfa gcde deafbg ce | ebfcga aecfbg cef abgcfe
eadcbg ecd gdebf fgeac cd dcfb fecdg fdgaecb cdfebg afebgd | gdfec adefbgc febgcd bgfadec
egfdca bfceg cfg cgabfe bfged bcadeg agebdcf cf bacf agbec | defgb cgbfade beagcf fgaceb
ebdga efabgc abcfdeg fe gfabcd dcfag fgaed ceafgd fge cfde | acbefgd agecfd edafg gadfcb
eadgc edagbf fbgd fadbce bg ebg ebdfa cfedabg cbfgea ebgad | aedbg egfdab cedbfa aecbfg
gd cefga cdabfge dgfce afbdgc dacfbe fgdebc cdg cbedf gebd | dg febdcga bedg gd
gedfb bcfgde cbfde cebg egf adbcef edgcaf cgdeabf eg gafbd | gdbef abecfd ge feg
bdacfg cba ab gcdefa cdbeg acgef egcfbda gafebc abfe caegb | cafeg bgcea gface agbec
fcabdge gdceab bdgcf ecfdb cgd cgef gc cfebdg dgfab adbfec | dgc gdcfbe gc fegc
cgbd gefbdc cdfeg gaecdf gfbedac cebdf faecb dbf db gedfab | cbdef bdf ebacgdf ecbfd
fdecbg bagcef beagc dbgfaec edagbc dgafb eafgb efg ef acfe | cgfaeb ceaf dbcgea gfbae
cfdae cefg acf bcaedg aecdg eadbgfc fc dbafe bfadgc defcag | efgc cfa ceadbg cfa
egc bgadef fcbdg cgdfe cdafge ce gadebc begcdfa faedg efca | efadg cdeafg cgfdb faegdb
ed agdbc bedcg cfageb edfb fgbdec ged dfaceg aedcgfb gfceb | de debf agdcb fdgaec
beadgfc bc fabc cbeda dbefa cfbead ecfgbd cbe dgeca ebagfd | dceba edcafb cadgbfe daceb
dgbcef afdbc fgdaeb ef edf edcgb eabcdg gbacfde cefg cfedb | ebfcd gbfedc bafcd ef
fecgb gbcfad aebdgc cgbaf cfedag gbcad afg fa afdb cafedgb | afdcge gdbacf acgdb gcdaeb
abd cfeadbg agedbf ba fdaeb fbga fcdae dabgce fcebdg edfbg | dfegcb dcebgf befdcg bfade
gbacfed gdbce gdaec fcedag eb cgfdb adbe ecb eadgcb fegcba | acebdg egbfac dcafge fbdgc
fbea fa acgbdf bdcgfe deacg daf fbged ceagbfd fgade gedfab | bcgfaed af af dfa
fedbgc dfgea gbdefa bagd egacbdf bceaf aegbf gb ecdgaf bgf | bg caedfg aefgdbc bfg
dabcgfe dbfce cgeda adcbe bgaced eab gadb fdgcea ba bcefga | cdbae ba cfaedg cdaeg
bed bagedc agebd feagcd edcga bdac bgfaedc fbega gebdfc db | debcfg cagbde egadc dceag
afgcb agef badcf ebfagc befcgd gfb fg fdagbec gbcea gecbda | gdcfeb bdfceg cdfba cegfdb
cdbegf abgfedc geabcf dgefa fc cagdbe cfe fagce cfab gabce | cfba acfb cf bagfedc
ecbga abedgc fce gbacfe aecdgbf afcb eagcf cgefdb fc adefg | afdeg dcfabeg bfca gdacbef
cedfg gaedb befcadg gaced cadfeg cabdfg ac adc befdcg ecfa | egbad cafgbd egfdcb dgecf
agbc cegda bdecfa edgfb gdbace adb ab bgfdace gcdefa ebdag | abedg ba gbdae geacd
cefbg gaed gadfb gfabdce egfdb edagbf ed dgbacf deb dfebca | ebd cefadgb ecfbgad bdacfg
ea adbfc defcgba eafb dgfbac edacbf fdcge ace fadec aedgbc | ea cfdea bagdce fdegc
cedfg cea dfabecg efgcab fcgea agbfcd feab facbg ae gadebc | bafgcd ea cdbeag gcafb
bgcd cb dfbac dcagefb gdcfab acefgb cbf acgfd adfegc bfaed | cbfgad fgacd cfdba gcbd
gebcfda dabcg bgdaef dgc cg gfbad eacdb fgbc cgfdba gfadce | agdfb befdag gcd dbagf
cbedgf fcg degacf agcdbe dbcaf gbdec dgbcf bgef fg fbadegc | gadefc adcbf acdbf fbcda
egadf agf dcga cbfedg ceafgb ga cdfaeg dafbe bacfedg ecgfd | feadbcg gcad agf fbead
agcbfd cdbge afecdg adfe gafce agd facgedb da cdega feagbc | faegbc gdcbfea cegfa cbfeag
egc bfcdea gbfe afcgeb ge aebcf adgcf adgcfeb dabgce agecf | cgabde cegaf fcaeg gfceba
bceda bea fdgcae eb eafcgbd egadc fcageb dbacge bdeg bcafd | bcfad edacb cgefda bgfaec
fgabd bdgefa cagdf gfb fbaceg efdb cbagde fb afgedcb edagb | gdfca gadeb agfdc ecdbga
ab gafdbc ceafg dgaecfb feagcb efcdag dbefg caeb baf efgba | abf abf agecbf fedgb
gbdface gbadce gaedc cfagbd cbdga eda ed gcfae bgfead decb | cbed fbdcage dbgac acbegd
fcda ecgbdf df abefd ceafbd cfageb dageb cabef dfe edacgfb | gedba eabgfc fd acgbef
dg egbafc fdbaecg decg fedab agcfdb cbgea cegadb egdba bdg | dg gaebc cdeg gedab
fcbd db adfgc fdbgca gaefb bcfadge dfgab bda dcebag gacfed | geadfc bd adb bfagdce
ecdgb beagc gbcedaf dcbgfe bgfadc faebc ag acgdbe egda cga | gceab eadg gaed dacgbf
degcbfa febdgc egca cgd dbcga gaefdb bdgae cabdf cg dbeacg | dcbgef deagbc baegcd acgdb
fbc gfdeba gbefca deafb decbaf agdcf cb cfdab edbc dcegafb | abgfed dfbca bcdfea dabfe
de gdefacb becdag begac eabfgc ebdc edg aedfbg cfagd caged | bcgfea deagcb bced cbed
bafc gbafed gfb fgaebc fb aegcf fcbaged afecgd befcg cbegd | gfaec fbagce agfedb cgebaf
edcgab dgfb dagef bcfgae fg eabdfg defca degba efg cgdbafe | bdage gdbea bfgcdea aegfdb
ceafg fca fcabge cgedf dbfacg af fgbcade cedgab abceg feab | eacgf fbcgda fca fdcgabe
gfacd gcbd afdbgc bafecg bc eabdf abc acdefg aegfcdb bfdca | bafcegd facbd bca cdgb
bdafge edfgcb cdf becf fgbdca efdgb egacd fc ecgdabf degfc | egabdf dbefga cafbedg fgcdeb
bd bcfagde fadb efbdc cedfab aecdgb eadfc bdc cgfbe gfdcae | dfab cbfeg bd efdca
cf gcfdab bgfadce cadegb adecgf fcd eacf eagdc fgedc fbged | gedbacf ecgad dfabgce degac
afbceg fge fe becga cfae febgdca fdgab fcbgde bcaegd baegf | baegf ef degcfb eacbg
gae cfgadbe feagc adgf adfbce fdcgea ga beagdc feadc gcebf | adfg eag edfca ga
ecdb cd cfadbe febadg agecbdf dfbae cfabg dcf afdbc aefdgc | dabfce adfbce cebd cdf
dcebaf ebdac ged bdag bfceg bdeagc bcdge ebdcagf dg gcfade | bdga fcegb gfcaedb gbedc
fcdeagb cgbd cgf abdfge cdfagb afbdg bafce bgcfa defcag cg | cgf cfdgea fdagce gfabc
becdgf gd bgfdca gfd dabcef gefca adbg facebdg afdgc bcdaf | acbdef dfacg dgf edgabcf
fbaegcd egcfa faebc cdabf efdb fabcgd cbe daecgb bcadfe eb | eb fgadcb dabfc dgceba
decafb cabfd fg ebcdfg facgd dagec fgba fbcedag gcf gacbdf | cadbf gedfcba fbacd cabfd
dgface dfabe dfaeg fg gdbeca fge gcdea gfceabd fcga efbgcd | becdga bcagde fagdec afcg
ceadb gdefbc cegdbaf adc bfdea cebdag acgb ca gcafed cbdeg | dac bcedag cbeda cabg
fdb dcgfaeb bgadef fbcdg bdce afcbg bd gfeadc dfgec fdgecb | bcagf bdaecfg ecfgadb bfd
gdefa bg egcafdb egbafd dbfegc afgb ecfgda abedc dbg dabeg | fdegbc eadgf afdeg gb
bfcad bag gbec agedc agfdceb gb fdagbe begdac gefacd gbcda | acfbd gba dcbag abcdf
aeg bdcega ge fgcda bdcae afegbd ecdga ecabdf egbc dbeagfc | ecabd edcbga agbfdec afdbeg
fgdce deca agfedc bdfeagc bdcfag eagfb dga cgdfeb adfge ad | gacedf agbfe aegdf fcegda
bafg ecfbag cabfe cadfeg fedcb eabcgd cagfebd agebc af fae | aegdbc afgedc feacb fa
bcgdfe cabg cfaedb gc cfg efbdagc fagdc eagdf fcadb dbagfc | cfgdeba efcgbd bcadf dgeaf
feadcbg egfba fg begfdc gafd debfa geabc bfaegd gfb ecbfad | cbagedf ecdbfag dfebcg gf
ea gbdeca egbdc bdcea aebg cbadf fdeacg cedgfb cedfgba aec | ceabd gbdaec ebag fadecg
bedca dcbaef dfcb feb baecdg gbacfde afegc bf ebfac ebfadg | ebdca gcdbea fbe geacf
bafgcd dcb cgead dcgbef afcbg db adcgebf bdfa gbecfa bcadg | gcbadfe agbdc gbafc bcd
cg cga edcfab ceadbg acdgfe gfcd gafce eafcd egfab dfbgeac | efagc bfeagdc fdebac aedbfc
ebg dafeb cdabg agdcbe geafdcb agec gfdbce bgdfac eg gebda | ebgad ge bceadg dbfea
cfdeg acebdg abcfdg befdg bgf beadg fcbgade fabged abef fb | decgf bfae fcdge fgdeba
gdfeab feadc abgc ebgdcf dcgbaf acfdb ba bafcdeg gdbcf bfa | cbdfa fbegda gadebf afdec
bfecdg dbf fgbce gdefba dbec gbfeac db cgdaf gbdcf agfdbec | febcg gcbdf eafdgb bfgec
caebg gdcfa fedcabg dce dafe cfbdag de egdac fgbced gecadf | fcgebda cgfad edc gbdcefa
edcgab dcfeabg dfbga afbdcg bdfe egfab deafbg egb eb feagc | badfeg bcgead eb abfdg
bc geacbf gebca bdgafe fbceda bce dfacegb degca cbgf abgef | bgacedf bcega gafdeb cgabe
gfdcaeb ceagf cgf dcag efcbdg ecadf cg efbag gdfaec bdfaec | eafgc cafeg gc dgcbfe
fdgcab bdgfc dcfgeab daefgc fgbdea bfgec gdb db cbad fadcg | gdb bgd dfcag bgaecdf
ebca fagedc cb bcefgd cefda acbfd deacfb bcf bafdg ecgfabd | cagedf fadec fdcae fdeca
dgacef deacb dac afgdecb fdcaeb dabeg acbf bedcgf ca debfc | bedag fgdcea acdbe bgead
bdcaef cdegfa egfdba cfd bfec egfbacd cfdba fc cgabd bafed | daebf bdacg cbfdgea bcdfa
ac cab ebgfa fcedb cbedgf dbcfea gbdfeca ecafb fcda gedbac | baefg ca ca bdcfe
afcbge fedbca fagebdc abdce dacegb aefd fcdbg fa cfa cabdf | af af beagfc abecd
edacg afdgce fcagedb degcb deb db faegbd abdgce bacd gfbec | egcdb dabc dabcge edb
cgdab efabdgc bae bacefg efgbc fcea fbcdeg bgeac ea aebfgd | ae cbgae dbfeag fcegb
acfed eg dgae fbceda gce dgface ebafgc fgdcb cedgf fdgcbea | fbcdg fdcaeb caegdfb cgabfe
dcgab cae ae eabgc gedfca defcgb fedacbg efbgc bfea eacgbf | cgdab ebaf bdcgfe eac
gafbced ceadfg gabfc ae cgbfae febcd cefba cfgadb bgea ace | fbdce eca bgafc afgdcb
bgd edfab acgfbd dg gdac dbfga bcgfa gdbcafe bdcefg ebfgca | abedf dafgb abgfce fagbec
adbegf bgace gdfae afc fbcaed gcdf cf faecgdb gaefc cdaefg | cfa ceafg abdcfe agefdb
fdgcb adg cbefdg cbfadg adgcb dcfage ga cgbfdea bfag aedbc | gdfcea fecdgb acbgd bgdfc
febgdc gb fbecd fbg cgeabf agfed bdfeg bedcaf fcgdaeb gdcb | febgd cfabed cedbf bdfge
dbgcf dfg dgcafb bdcag df efdacg dcabeg facegdb adfb gbfce | fgcdb dcfgb agbdcef gbcef
abfdcg gbfea ceabgdf gedcbf ec bec cgfdb bgfce ecdf acgebd | cagdfb ebc dfgceb ce
bdafc fbaeg dcagfb cgdb adg dg daefbc bdgaf dacfebg efadcg | dbgc bcfad aegfb adbcf
cdagfe cdegb dbe defcg gabec gfdcbae bd efbdgc bcadfe dbfg | eacfgd db db dbe
fbcegd bdfgc cf gbecafd gdcba fbc fgdeb cdef gadbfe acbefg | gdbfe gadfeb dbgfc bdfgec
abcegf cbfad eb fabcedg efabc ceb gfbe degafc gfeac abgced | agbcde ceb gfbe beacf
fgdc gbf fgcbda cefbga facdb gadbe gf bafdec gbdfa aebgfcd | fbg gdcbaf efcbad fgaecbd
ecadbf dcfbga bcdgeaf afgc adgcb bcf fbgde cf dgbaec cbfdg | eafbcd cf abcdgf dfebg
beafg edfgba fbacgd fcbeg ebcagd bdagf ea bae efad cgbfdea | afebcgd cfgbad efcbg bgdeaf
faegcb gefdb gdefcb dfcga dbfga fgadcbe ba bfeadg adeb abf | bfagd fgbedc ebfdg gfcdbae
adebc gacdeb gba aegbd cgefba ebfgdac acfdeb agdc ga bfdeg | ecadb dbefg fbgcae gcdafeb
gf dfbgec cdbef dgfb gcebdaf efbgc aefdcg bdaecf gef caebg | cdafgbe gfe cgbfade fbcge
cba ebfga ac agbdfec becgd cabdeg fdebgc daec gfbdac bagec | ca ebgcd ecbdgf fgdbce
gcafeb dbceg edga bcgdaef de bcfaed ced abecgd becga dcbgf | agbcfe aedg ed aegbcf
cbeg bcafg bgdaf ecgfba agc beafc ceagdbf cdegfa cg dfecab | gebc debcagf cg gc
dbaecgf gacbf gce bcedag bcgef dfceb ge efbdac degf dcfgbe | ecg defg ecfgb bfecd
ga gfceab bgdeafc acdbf bgcfe cbedga fgae bafcg fecbgd gba | acgfb ebadcg fcbad bcagf
cadb edbagf gdcbfa dgc cdfga adfbceg fagbd fgeca dcbegf cd | gbfedc cdebfag debfcg fagbd
fbcgea bacgedf gbce cdfagb eba be gbcfa egdabf cbafe acefd | eacbfg abe cegfba ecgb
bfcag gdba ba adfegbc eafbdc bfcdg ecagf bac fbgcad gcdfbe | bgadcf ebgcfd acb gbfced
ced geabcd dcaebgf fgaecb begac cadb cd cgdefa bdgec dfbeg | beacdg bdaceg fgcaedb cbeagd
gdacfb bfdgc gfaec ebfdcag fegbc cfgedb acdfeb gedb be cbe | bgfec be gbed egcaf
efcda abf gfdaeb gedcbfa abcdg bf bcdfa cfeb eagfdc caefbd | gabdc fecdag bgdfeca bdegaf
efcab gefcadb dcbge cagebd gfdb feacgd efd bdcefg df befdc | fbdce acfeb cdebg fd
gafbecd dacg efgbd dcfge ecdaf gcf cedfag febcga cg dbcefa | fdaebc gc afgceb gecfba
ebgcd fgba dbcgaf cgbad gac gdbceaf cfdba cgaedf afdceb ag | abgdfec cegdb cag fbag
ecbgf efac bgade bacedgf begca agbcef gbfedc agc fcabdg ac | aefc fbcadg afbcgd dgcaefb
fab cafgbe face acgbedf eagbc bdgafe egcabd fbgca gdbcf fa | cfbgdea acbge cagbe fcea
fedac ec gced bacgfe acdbf begadf gedbfac cae fdaeg cgdaef | dfcgea dafge aefgdb fadge
gbcade fe afbgc cedfga bdfe gbfae fae fdbega dgbae cdbagef | egabf aef abgef efbd
fagebc fad dcfab acdbfge ceabf egadbf edfc dbafec fd gdbca | bafec dbgafe fd eafcb
afg ecdbag fcage dbgefa abefc gfdc fgcbdae fg fdecga gdeca | deacg fagdce cefag edfgca
badfc gcfdea db fedcgb abde bcd fdecgab deafc facedb fgabc | fbcgde abgcf gabfc aedgcf
bdaefc daecb gfcad aebg edgbac gbfcde bg cbadfeg gcb dbcga | bega cfabed cedba bgae
afdbec ge aeg cdagb egfb cebadfg egbda agfdeb facdge efbad | fdbeac agbdc ega cagdb
cfe eagfd bacfg egdfba edac gefca gecdafb eacdgf egcdfb ce | cfgbde fbgca efdbga daec
abfcg bdcegaf ecbfg bafecd gcda dfcab cagfbd edgabf ga gab | dacg gacd gacd bfgca
egbac ec cbdgfa ceb agdcb bagedc cdbaefg gbaef fcbade decg | dfacbg bafge ce efbag
bd bdafce gcdfe fdcbg bfgca dfbceg dbf gecfda dgbcfea gdbe | edfcg fcdge fbcgde gcedf
eafcbdg eagbc gcaebd cfegab dfeca bgfedc cfb befac bfga bf | bgdace bacfe fb feagbc
bgedfc bgafedc adc bafec gcdeba egbcd dbeca gcafbd gead da | gdefcb gecdab cbfged gcbde
bgec eb fbgea aeb ecagf dabgf cdfage bfdcae cebgdfa fabceg | cgadef begacf afgbe egcfdab
fe fdecabg ebcfa bgcaf ebcgfa aebdc begf fcedga fdcbag aef | bacde fae fegb efa
gdefbac dgfecb afdg gcfbad gcd ecabg gd afcdb bfdcea gdcab | acdgb dg dcg dafg
bcaefd aceb dagecf ea cgebafd eda ebgdf faedb dbafc bgdcaf | facbdg aecb aebc gdefb
cafdeg egcfb ge afedcb egba acgfbe cebadfg gbfdc baefc egc | ebga ge ecg aecfb";
    }
}
