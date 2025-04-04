export default interface Transaction {
  id: string;
  description: string;
  transactionType: string;
  date: string;
  amount: number;
  sourceWallet: {
    id: string;
    name: string;
    balance: number;
    userName: string;
  };
  destinationWallet: {
    id: string;
    name: string;
    balance: number;
    userName: string;
  };
}
