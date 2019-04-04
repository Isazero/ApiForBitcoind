# ApiForBitcoind
This is a test task for work in Atlant company

I worked with regtest configuration of bitcoind. 

It has to main methods SendBtc and GetLast.

SendBtc sends some amount of bitcoins to the wallet address and returns transaction Id. Also, this method adds information
about transaction to Database.

To use SendBtc method you should make a post request which contains next parameters : 
Address(To which send the bitcoins), Amount, Username and Password

I added Username and Password for security. Default username and password are initialized in BitcoinApiContext class.

GetLast shows transactions that have less than 3 confirmations.

For any questions contact with me: ikuliev98@gmail.com or https://t.me/KickIon


