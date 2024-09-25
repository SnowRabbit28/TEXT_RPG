using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks.Dataflow;


namespace TxtRPG
{

    /* 1- 1. 캐릭터 정보를 가져온다.*/
    /* 캐릭터 생성 클래스*/
    public class PlayerInfo
    {
       
        public int level { get; set; }
        public string job { get; set; }
        public int ad { get; set; }
        public int dep { get; set; }
        public int hp { get; set; }
        public int gold { get; set; }

        //인벤토리 아이템을 리스트로 선언
        public List<Item> Initem { get; set; }

        //이것은 생성자 인가?
        public PlayerInfo( int _level, string _job, int _ad, int _dep, int _hp, int _gold)
        {
            level = _level;
            job = _job;
            ad = _ad; //  공격력
            dep = _dep; // 방어력
            hp = _hp;
            gold = _gold;
            Initem = new List<Item>();
        }
        /* 상태정보에 들어갈 정보 */
        public void PlayerIn() // [E]가 
        {
            // 아이템 능력에 따라 값이 바뀌어야해서 따로 설정
            string ad_text = $"{ad}"; //10
            string dep_text = $"{dep}";//5

            if (ad != 10) ad_text = $"{ad} (+{ad - 10})"; // 17 (+7)
            if (dep != 5) dep_text = $"{dep} (+{dep - 5})";
            Console.WriteLine($" 레벨은 {level}레벨 \n 직업은 {job} \n 공격력은 {ad_text} \n 방어력은 {dep_text}\n 체력은 {hp}\n 골드는 {gold}\n");
        }
        //
        public void BuyItem(Item buyItem)
        {
            Initem.Add(buyItem);
        }
    }

    public class Item //클래스는 구조체랑 다르잖아
    {
        public string ItemName { get; set; }
        public string ItemDes { get; set; }
        public string ItemType { get; set; }
        public int ItemPower { get; set; }
        public int ItemPri { get; set; }
        public int ItemPurchase { get; set; }
        public bool ItemEquip { get; set; }


        public Item(string itemName, string itemDes, int itemPri, int itemPurachae, bool itemEquip, string itemType = "", int itemPower = 0)
        {
            ItemName = itemName;
            ItemDes = itemDes;
            ItemType = itemType;
            ItemPower = itemPower;
            ItemPri = itemPri;
            ItemPurchase = itemPurachae;
            ItemEquip = itemEquip;

            if (itemPurachae != 0) Console.WriteLine(itemName + " | " + itemDes + " | " + itemType + " +" + itemPower + " | " + "구매함");
            else Console.WriteLine(itemName + " | " + itemDes + " | " + itemType + " +" + itemPower + " | " + itemPri + "G");
        }
    }

    internal class Program
    {
        
        static PlayerInfo charInfo;
        static string[] playitem = new string[6];
        static string[] itemName = new string[] { "수련자의 갑옷", "무쇠갑옷", "스파르타의 갑옷", "낡은 검", "청동 도끼", "스파르타의 창" };
        static string[] itemPower = new string[] { "방어력 + 5", "방어력 + 9", "방어력 + 15", "공격력 + 2", "공격력 + 5", "공격력 + 7" };
        static string[] itemDes = new string[] { "수련에 도움을 주는 갑옷입니다.", "무쇠로 만들어져 튼튼한 갑옷입니다.", "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", "쉽게 볼 수 있는 낡은 검 입니다.", "어디선가 사용됐던거 같은 도끼입니다.", "스파르타의 전사들이 사용했다는 전설의 창입니다." };
        static int[] itemPay = new int[] { 1000, 1500, 3500, 600, 1500, 6000 };
        static bool[] buy = new bool[6] { false, false, false, false, false, false };
        static void Main(string[] args)
        {
            charInfo = new PlayerInfo(1, "전사", 10, 5, 100, 1500);
            //시작화면 처음에 바로 나와야하니까 main에 작성
            Console.WriteLine("\t★ 텍스트 RPG 던전에 오신걸 환영합니다.★");
            Console.WriteLine("여기는 던전으로 가시기 직전, 쉬어가는 마을 텍리지입니다.");
            Console.WriteLine("\n");

            getStart();

        }
        /* 처음 시작시 뜨게 하는것! */
        static public void getStart()
        {
            // 이 코드도 지워줘? .응 z 키 누르면 그만이야
            while (true)
            {   //시작글
                Console.WriteLine("1. 내 상태 확인");
                Console.WriteLine("2. 가방 열기");
                Console.WriteLine("3. 상점 가기");
                Console.WriteLine("\n");
                Console.Write("어떤 행동을 하실건가요?   ");

                //TryParse : 문자열 -> 정수/ 변환안되면 false 출력
                if (int.TryParse(Console.ReadLine(), out int startNum))
                {
                    switch (startNum)
                    {
                        case 1: // 상태정보
                            PlayerInfo();
                            break;

                        case 2: // 가방
                            Inventory();
                            break;
                        case 3: // 상점
                            Store();
                            break;
                    }
                    // 만약에 제대로 된 숫자 입력시 다음 함수로 넘어가야하니 루프종료
                    if (startNum >= 1 && startNum <= 3)
                    {
                        break;
                    }
                    else
                    {//123이 아닌 숫자를 누르면 출력
                        Console.Clear();
                        Console.WriteLine("음...거긴 아직 못가요! \n 1부터 3까지 사이를 눌러주세요\n\n");
                    }
                }
            }


        }

        /* 0. 나가기 만들기, 나간다는건 전으로 돌아가게끔 */
        static public void ExitHome()
        {
            Console.Clear(); // 위의 글 지우기
            if (int.TryParse(Console.ReadLine(), out int startNum))
            {
                getStart();
            }
        }


        /* 1.상태 정보 */
        static void PlayerInfo()
        {

            Console.Clear(); // 위의 글 지우기

            while (true)
            {

                Console.WriteLine(" ☆상태 정보를 표현해줍니다.☆\n");
                Console.WriteLine("=========================\n");
                charInfo.PlayerIn(); // 정보를 담은 클래스 => 이부분을 좀더 간단하게 수정할 방법이 있을까..?
                Console.WriteLine("=========================\n");
                Console.WriteLine("0. 뒤로 돌아가기\n\n");

                Console.Write("원하시는 행동을 선택해주세요.  ");


                //여기서는 정보를 보고 돌아가는거 뿐이니..
                if (int.TryParse(Console.ReadLine(), out int infoNum))
                {

                    if (infoNum == 0) //0을 누르면 돌아간다.
                    {
                        Console.Clear();
                        Console.WriteLine("돌아갑니다.\n\n");
                        getStart();
                        break;
                    }
                    else if (infoNum != 0) // 번호 잘못눌렀을때,
                    {
                        WrongNum();
                    }
                }

            }

        }


        /* 2.가방 진입 */
        static void Inventory()
        {

            Console.Clear(); // 위의 글 지우기
            while (true)
            {

                Console.WriteLine("\t☆가방입니다.☆");
                Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n\n");

                Console.WriteLine("[ 아이템 목록 ]\n");

                for (int i = 0; i < charInfo.Initem.Count; i++)
                {
                    string equipment = "";
                    Item p_item = charInfo.Initem[i];
                    if (p_item.ItemEquip) equipment = "[E]";
                    Console.WriteLine("- " + equipment + p_item.ItemName + " | " + p_item.ItemType + " + " + p_item.ItemPower + " | " + p_item.ItemDes);
                }

                Console.WriteLine();
                Console.WriteLine("1. 장착 관리");
                Console.WriteLine("0. 나가기\n\n");

                Console.Write(" 원하시는 행동을 선택해주세요.  ");

                if (int.TryParse(Console.ReadLine(), out int infoNum))
                {

                    if (infoNum == 1)
                    {
                        equip_item();
                    }
                    else if (infoNum == 0) //0을 누르면 돌아간다.
                    {
                        Console.Clear();
                        Console.WriteLine("돌아갑니다.\n\n");
                        getStart();
                        break;
                    }
                    else // 번호 잘못눌렀을때,
                    {
                        WrongNum();

                    }
                }

            }

        }

        static void equip_item()
        {
            Console.Clear(); // 위의 글 지우기
            while (true)
            {

                Console.WriteLine("\t☆가방입니다.☆");
                Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n\n");

                Console.WriteLine("[ 아이템 목록 ]\n");

                for (int i = 0; i < charInfo.Initem.Count; i++)
                {
                    string equipment = "";
                    Item p_item = charInfo.Initem[i];
                    if (p_item.ItemEquip) equipment = "[E]";
                    Console.WriteLine("- " + (i + 1) + ". " + equipment + p_item.ItemName + " | " + p_item.ItemType + " + " + p_item.ItemPower + " | " + p_item.ItemDes);
                }

                Console.WriteLine();
                Console.WriteLine("0. 나가기\n\n");

                Console.Write(" 장착할 장비를 선택해주세요.  ");

                if (int.TryParse(Console.ReadLine(), out int infoNum))
                {
                    if (infoNum > 0 && infoNum < 6)
                    {
                        if (!charInfo.Initem[infoNum - 1].ItemEquip)
                        {
                            charInfo.Initem[infoNum - 1].ItemEquip = true;
                            Item p_item = charInfo.Initem[infoNum - 1]; //인덱스는 0번인데 나는 1번을 눌러주어야하니까 쟤를 호출하면 -1으로 첫번째가 나와야하네
                            if (p_item.ItemType == "공격력") charInfo.ad += p_item.ItemPower;
                            if (p_item.ItemType == "방어력") charInfo.dep += p_item.ItemPower;
                        }
                        else
                        {
                            charInfo.Initem[infoNum - 1].ItemEquip = false;
                            Item p_item = charInfo.Initem[infoNum - 1];
                            if (p_item.ItemType == "공격력") charInfo.ad -= p_item.ItemPower;
                            if (p_item.ItemType == "방어력") charInfo.dep -= p_item.ItemPower;
                        }

                        equip_item();
                    }
                    else if (infoNum == 0) //0을 누르면 돌아간다.
                    {
                        Console.Clear();
                        Console.WriteLine("돌아갑니다.\n\n");
                        getStart();
                        break;
                    }
                    else // 번호 잘못눌렀을때,
                    {
                        WrongNum();

                    }
                }

            }
        }


        static void Print_ItemList(bool print_type, PlayerInfo player)
        {

            for (int i = 0; i < charInfo.Initem.Count; i++)
            {
                buy[player.Initem[i].ItemPurchase] = true;
            }
            for (int i = 0; i < 6; i++)
            {
                string[] power = itemPower[i].Split(" ");
                int purchase = 0;
                // 예) power = ["공격력", "+", "3"]
                // 방어력이 있는 경우
                if (print_type) Console.Write("- " + (i + 1) + ". ");
                if (!print_type) Console.Write("- ");
                if (buy[i]) purchase = i + 1;

                if (power[0] == "방어력")
                {
                    Item item = new Item(itemName[i], itemDes[i], itemPay[i], purchase, false, power[0], int.Parse(power[2]));
                }
                // 공격력이 있는 경우
                if (power[0] == "공격력")
                {
                    Item item = new Item(itemName[i], itemDes[i], itemPay[i], purchase, false, power[0], int.Parse(power[2]));
                }
            }

        }
        /* 3. 상점 진입 */
        static void Store()
        {
            Console.Clear(); // 위의 글 지우기


            Console.WriteLine("\t상점에 들어옵니다.");
            Console.WriteLine("던전에 필요한 물건을 구입하세요!\n");
            Console.WriteLine($"[보유골드] \n {charInfo.gold}G \n\n");

            Console.WriteLine("[아이템 목록]");
            Print_ItemList(false, charInfo);

            Console.WriteLine();

            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("0. 나가기\n\n");

            Console.Write(" 원하시는 행동을 선택해주세요.  ");

            if (int.TryParse(Console.ReadLine(), out int infoNum))
            {
                while (true)
                {
                    if (infoNum == 1)
                    {
                        BuyItem(false, false);
                        break;
                    }
                    else if (infoNum == 0) //0을 누르면 돌아간다.
                    {
                        Console.Clear();
                        Console.WriteLine("돌아갑니다.\n\n");
                        getStart();
                        break;
                    }
                    else
                    {
                        WrongNum();
                    }
                }

            }


        }

        /* 3-1 상점안에 아이템 목록 */
        static void BuyItem(bool noMoney, bool noBuy)
        {
            Console.Clear(); // 위의 글 지우기


            Console.WriteLine("\t상점에 들어옵니다.");
            Console.WriteLine("던전에 필요한 물건을 구입하세요!\n");
            Console.WriteLine($"[보유골드] \n {charInfo.gold}G \n\n");

            Console.WriteLine("[아이템 목록]");

            Print_ItemList(true, charInfo);

            Console.WriteLine("\n0. 나가기\n\n");
            if (noMoney) Console.WriteLine("==========돈이 부족합니다.========\n");
            if (noBuy) Console.WriteLine("========이미 구매한 아이템입니다.=========\n");

            Console.Write(" 구매하실 아이템 번호를 입력해주세요.  ");

            if (int.TryParse(Console.ReadLine(), out int infoNum))
            {

                if (infoNum < 7 && infoNum > 0)
                {
                    string name = itemName[infoNum - 1];
                    if (charInfo.Initem.FindIndex(c => c.ItemName == name) != -1)
                    {
                        noBuy = true;
                        noMoney = false;
                    }
                    else if (charInfo.gold < itemPay[infoNum - 1])
                    {
                        noMoney = true;
                        noBuy = false;
                    }
                    else
                    {
                        string[] info = itemPower[infoNum - 1].Split(" ");
                        Item buyingItem = new Item(itemName[infoNum - 1], itemDes[infoNum - 1], itemPay[infoNum - 1], infoNum - 1, false, info[0], int.Parse(info[2]));
                        charInfo.BuyItem(buyingItem);
                        charInfo.gold -= itemPay[infoNum - 1];
                        noMoney = false;
                        noBuy = false;
                    }
                    BuyItem(noMoney, noBuy);
                }
                else if (infoNum == 0) //0을 누르면 돌아간다.
                {
                    Console.Clear();
                    Console.WriteLine("돌아갑니다.\n\n");
                    getStart();
                }
                else
                {
                    WrongNum();
                }

            }
        }


        /* 번호 잘못 눌렀을때*/
        public static void WrongNum()
        {
            Console.Clear();
            Console.WriteLine("잘못 누르셨군요! 번호는 0밖에 누를게 없습니다\n");
            Console.WriteLine("이 메시지는 2초뒤에 사라집니다.\n");
            Thread.Sleep(1500);
            Console.Clear();
        }
    }

    //이제 내가 해야하는게....
    // 1. 정보창...이 템을 끼면 + - 이런거
    // 2. 상점에서 물건사기
    // 2-1 물건을 샀을때, 번호로 바뀌고, 구매할거 고르라는 창 + 구매 완료 금액 차감 드읃으듣등아아악아ㅏ악아악ㄱ아각
    // 3. 장착을 누르면 [e]버튼 활성화 / 비활성화

}



