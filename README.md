# Knowledge Base

This project is the result of my first experiece with UWP. This app should help to organize your notes. Now it has two options: dictionary and notes page.

## Dictionary

Dictionary page has two list of words and card frame.

<img src="https://user-images.githubusercontent.com/56796561/220498103-db99f18f-d90f-4b03-8c0b-9f1e946a9348.png" width="800" height="400">

List in top cantains all words in dictionary, list under contains words you googled but not yet added to dictionary for whatever reason.

<img src="https://user-images.githubusercontent.com/56796561/220499116-7813fb4a-0af9-480b-a276-f1822e7f7da4.png" width="400" height="500">

Frame right to lists gives an opportunity to see and edit words in dictionary (use button with eye for switching between reading and writing mode). 
Also if you search some word in your dictionary and it doesn't exist you may press "Enter" and online dictionary will be opened. This may help you to know meaning of this word as quickly as possible. Them if you press save button watching word in browser this word will be saved in bottom words list.
Of course, this control helps us to add new words in dictionary and save all changes.
There you may see button with camera icon. If you are in reading mode and want to take screeshot of this card you should press this button. Image location you may see in Settings/History.

<img src="https://user-images.githubusercontent.com/56796561/220499830-bc14c578-5e99-40cf-8822-0cbe08968e4b.png" width="300" height="400">

## Notes

This page is necessary when you're watching some movie, learning how to play guitar or even listening some padcast. In all this situations you'll be happy having an opportuinty quickly make notes connected to particular media source.

It also has two lists but in horizontal orientation and source card below.

<img src="https://user-images.githubusercontent.com/56796561/220502264-832c0284-cefc-4223-807d-863c6b8f1069.png" width="800" height="400">
 
Every source has required fields (like name, duration, theme, description...) but you may add your own in extras. Use "Key" for naming field and "Value" to fill it.
There is one non-deleting source for saving all quotes and stamps which have no source link or it's unknown. When you delete some source all notes and quotes will be saved there.

<img src="https://user-images.githubusercontent.com/56796561/220503313-f8ea6f34-ba06-4375-8bed-b9b886a1fbb2.png" width="800" height="400">

You may also take a picture of every single note and quote if you need. Path will be in Settings.

## Settings

Now Settings has not much options. It has options for dictionary cards colors, history, synchronization and some files options.
For example, dictionary colors settings:

<img src="https://user-images.githubusercontent.com/56796561/220504754-a28f2788-802a-46c6-875b-e970a4b51721.png" width="800" height="400">

## How it works with real data
If you want to see how it works please follow this instructions:
1. When you first open this app it will create some files and folders in "LocalState" folder on your device. We need to know where it is to copy example databeses there.
2. Open notes page in the left toolbar. Select "VIDEOS" in left list and select "UNKNOWN_SOURCE".
3. Then scroll your page down to see card. At the bottom right corner you will see some buttons. Click button with eye icon. You will change your mode from "Read" to "Edit".
4. Click button save and accept saving. It will create markdown file in "LocalState" folder.
5. Open settings in the left toolbar. Open History. Copy file path and find "LocalState" folder using it.
6. You should replace databases in "LocalState" folder with example databases. Example databases are in root folder of this project.
7. After this actions you have an opportunity to see how it looks like with real data.

## Afterwords

If you think that this app has to many error, code is not looking super-duper nice, you are right. I'm working on it, trying to do my best. Also I'd like to fild a job connected with Xamarin, UWP or WPF as intern/trainee.
If you have an offer for me or recomendstions how to improve this app I would be glad to receive a letter from you on my email: schifrindenis@gmail.com.
