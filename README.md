# SaveStates
Quick save/load for Phoenotopia: Awakening using Unity Mod Manager
<img src="https://raw.githubusercontent.com/Gyoshi/PhoA-SaveStates/master/Resources/logo.png" alt="Not actual gameplay footage" align="right"/>

***While holding the camera button:***
- Press the right analog stick (while centered) to make a **Quicksave**
- Press RB (Grab) to **Quickload**
- Press LT/RT to swap between save **Slots**
- Press Start (Inventory) to skip ahead by **10 Slots**
- Press Select (Options) to jump to the **Autosave Slot** (0)
- Press the Alt Tool button to use the regular camera zoom feature
- On keyboard, the following keys also work (while holding camera): 
	- Home/End to **Quicksave**/**Quickload**
	- PgUp/PgDn to swap between **Slots**

You can swap between 99 Quicksave Slots, and 1 Autosave. The save data of each Quicksave Slot is stored as a file in the mod folder.

*This tool is still under development.*

下に日本語でもあります。

## Autosaving
A Quicksave will automatically made each time you move between levels. The Autosave Slot is accessible before Slot 1.

## State preservation
Everything related to Gail and her progress through the story is preserved across quicksaves and -loads. Most of the state of the npcs and objects in a room is not. More precisely:

This utility saves and loads
- Location
- Inventory
- Health/energy state (including buuffs)
- Game state that is transferred across normal saves (progression flags, chrysalis locations, etc.)
- State of doors, boxes

Does not currently save
- Gail's exact state (velocity, rolling/swimming/climbing, etc.)
- State of NPCs
- Location of Boxes
- Projectiles, Bombs
- Pause/Inv menu
- Cutscene/dialogue progress

## From Title Screen/Main Menu
You can also quickload from the opening menu. Beware that it defaults to the first regular save file, so if you do a regular save after quickloading from the opening menu, it will overwrite whatever was in the first save file.

## Installation
Requires [Unity Mod Manager](https://www.nexusmods.com/site/mods/21/). You just need the `PhoA-SaveStates.zip` file [(which you can download here)](https://github.com/Gyoshi/PhoA-SaveStates/releases/latest), and then follow the mod installation instructions for Unity Mod Manager.

## Known Issues
- Doesn't properly override cutscenes that move Gail or the camera.
- Sneak attacks might be less reliable after quickloading.
- The Boss health bar might not display correctly after quickloading.
- Background visuals and music don't update when quickloading some rooms.

---
Feel free to @Gyoshi on the Discord [Phoenotopia Fan Server](https://discord.gg/Swd6zcTCQZ) for whatever

For more info about what I plan to work on, you can look at the [PhoA SaveStates Trello board](https://trello.com/b/LoMwIPi0/phoa-savestates). I don't really expect anyone to be that interested, but I put up a pretty background, so I felt like sharing

source code: https://github.com/Gyoshi/PhoA-SaveStates

---
# SaveStates
フェノトピアをプレイ中に使えるクイックセーブ＆ロードのツール
<img src="https://raw.githubusercontent.com/Gyoshi/PhoA-SaveStates/master/Resources/logo.png" alt="実際のゲームプレイ映像ではありません" align="right"/>。

***カメラボタンを押したまま：***
- 右アナログスティック（中央）を押すと、**クイックセーブ**ができます
- RB（グラブ）を押して**クイックロード**します
- LT/RTを押して、どの**スロット**にセーブとロードするかを選択することができます
- ＋（インベントリー）を押すと、**10 Slots**前に進みます
- －（オプション）を押すと、**オートセーブスロット**（0）にジャンプします
- Yボタンを押すと、通常のカメラのズーム機能が使えます
- キーボードでは、以下のキーも機能します（カメラボタンを押したまま）
	- Home/End は**クイックセーブ**/*クイックロード**
	- PgUp/PgDnで**スロット**を選択することができます

99個のクイックセーブスロットと1個のオートセーブスロットを交換することができます。各クイックセーブスロットのセーブデータは、MODフォルダにファイルとして保存されます。

*このツールはまだ開発中です。*

## オートセーブ
レベル間を移動するたびにクイックセーブが自動的に作成されます。オートセーブスロットは、スロット1の前にアクセスできます。

## 状態保存
ゲイルと彼女のストーリーの進行に関連するものはすべて、クイックセーブとロードは保存します。部屋の中のNPCやオブジェクトの状態は、ほとんど保存されません。

このユーティリティは、以下をセーブとロードをします。
- 所在地
- インベントリー
- HP・スタミナ状態（バフを含む）
- 通常のセーブで引き継がれるゲーム状態（進行フラグ、クリスタリスの位置など）。
- ドアや箱の状態

現在、以下をセーブしません。
- ゲイルの正確な状態（速度、ローリング/スイミング/クライミングなど）。
- NPCの状態
- 箱の位置
- 投射物、ボム
- メニュー状態
- カットシーン/ダイアログの進行

## タイトル画面/メインメニューから
オープニングメニューからクイックロードすることもできます。ただし、デフォルトでい一位のセーブファイルに保存されるので、オープニングメニューからクイックロードした後に通常の保存を行うと、最初に保存したファイルにあったものが上書きされることに注意を。

## インストール
[Unity Mod Manager](https://www.nexusmods.com/site/mods/21/)が必要です。その後 `PhoA-SaveStates.zip`というファイルを[ここからダウンロードして](https://github.com/Gyoshi/PhoA-SaveStates/releases/latest)、Unity Mod ManagerのMODインストール手順に従うだけです。

## 既知の問題
- ゲイルやカメラを動かすカットシーンが正しく上書きされない。
- クイックロード後、スニークアタックの信頼性が低下する場合があります。
- クイックロード後、ボスのヘルスバーが正しく表示されない場合があります。
- 一部の部屋をクイックロードすると、背景のビジュアルと音楽が更新されない。

---
Discordの[Phoenotopia Fan Server](https://discord.gg/Swd6zcTCQZ)に@Gyoshiでお気軽に何でもどうぞ。（日本語ならダイレクトメッセージの方がいいかな）

[PhoA SaveStates Trello board](https://trello.com/b/LoMwIPi0/phoa-savestates)で開発進行を確認できます

ソースコード: https://github.com/Gyoshi/PhoA-SaveStates
