"use client";

import { useState, useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { FaArrowLeft, FaArrowRight } from "react-icons/fa";
import {
  Dialog,
  DialogPanel,
  DialogTitle,
  Transition,
} from "@headlessui/react";
import { Fragment } from "react";
import { Toaster } from "react-hot-toast";
import Skeleton from "react-loading-skeleton";
import "react-loading-skeleton/dist/skeleton.css";
import Wallet from "../models/wallet";
import Transaction from "../models/transaction";
import toast from "react-hot-toast";

export default function TransactionPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [loading, setLoading] = useState(true);
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [token] = useState<string | null>(localStorage.getItem("token"));
  const [wallets, setWallets] = useState<Wallet[]>([]);
  const walletId = searchParams.get("walletId");
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [total, setTotal] = useState(0);
  const [newTransaction, setNewTransaction] = useState({
    amount: 0,
    description: "",
    sourceWalletId: "",
    destinationWalletId: "",
  });
  const [pagination, setPagination] = useState({
    skip: 0,
    take: 5,
    walletId: walletId,
  });

  const fetchTransactions = async () => {
    setLoading(true);
    try {
      const response = await fetch(
        `http://localhost:8080/api/transaction/wallet/pagination?skip=${pagination.skip}&take=${pagination.take}&walletId=${walletId}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      const data = await response.json();
      setTransactions(data.data);
      setTotal(data.totalCount);
    } catch (error) {
      console.error("Erro ao buscar transações:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (walletId) fetchTransactions();
  }, []);

  useEffect(() => {
    if (showCreateModal) {
      fetch("http://localhost:8080/api/wallet", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
        .then((response) => response.json())
        .then((data) => setWallets(data.data))
        .catch((error) => console.error("Erro ao buscar carteiras:", error));
    }
  }, [showCreateModal]);

  useEffect(() => {
    fetchTransactions();
  }, [pagination]);

  const showToastMessage = (success: boolean) => {
    if (success) {
      toast.success("Mensagem exibida com sucesso!", {
        style: {
          border: "1px solid #10B981",
          padding: "16px",
          color: "#10B981",
          backgroundColor: "#ECFDF5",
        },
        iconTheme: {
          primary: "#10B981",
          secondary: "#ECFDF5",
        },
      });
    } else {
      toast.error("Ocorreu um erro, tente novamente!", {
        style: {
          border: "1px solid #EF4444", // Vermelho para erro
          padding: "16px",
          color: "#EF4444",
          backgroundColor: "#FEF2F2", // Fundo claro com tom de erro
        },
        iconTheme: {
          primary: "#EF4444", // Cor do ícone de erro
          secondary: "#FEF2F2", // Fundo do ícone
        },
        duration: 4000, // Duração de 4 segundos
      });
    }
  };

  const handleCreateTransaction = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const response = await fetch("http://localhost:8080/api/transaction", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ ...newTransaction, sourceWalletId: walletId }),
      });
      if (!response.ok) throw new Error("Erro ao criar transação");

      showToastMessage(true);
      fetchTransactions();
      setShowCreateModal(false);
      setNewTransaction({
        amount: 0,
        description: "",
        sourceWalletId: "",
        destinationWalletId: "",
      });
    } catch {
      showToastMessage(false);
    }

    //setTimeout(() => setMessage(null), 5000);
  };

  const tryParseFloat = (value: string, defaultValue: number = 0): number => {
    const parsed = parseFloat(value);
    return isNaN(parsed) ? defaultValue : parsed;
  };

  const handlePageChange = (direction: string) => {
    setPagination((pagination) => ({
      ...pagination,
      skip:
        direction === "next"
          ? pagination.skip + pagination.take
          : Math.max(0, pagination.skip - pagination.take),
    }));

    fetchTransactions();
  };

  return (
    <>
      <div className="p-6 max-w-4xl mx-auto bg-gray-50 min-h-screen">
        <h1 className="text-3xl font-bold text-gray-800 mb-6">
          Transações da Carteira
        </h1>

        {/* Botões de Ação */}
        <div className="flex gap-4 mb-6">
          <button
            onClick={() => router.push("/wallet")}
            className="flex items-center gap-2 bg-white text-indigo-600 px-4 py-2 rounded-lg border border-indigo-600 hover:bg-indigo-50 transition-colors duration-200"
          >
            <FaArrowLeft />
            Voltar
          </button>
          <button
            onClick={() => setShowCreateModal(!showCreateModal)}
            className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-colors duration-200"
          >
            {showCreateModal ? "Cancelar" : "Nova Transação"}
          </button>
        </div>

        {/* Modal de Criação */}
        <Transition appear show={showCreateModal} as={Fragment}>
          <Dialog
            as="div"
            className="relative z-10"
            onClose={() => setShowCreateModal(false)}
          >
            <div className="fixed inset-0 bg-black bg-opacity-30"></div>
            <div className="fixed inset-0 flex items-center justify-center">
              <DialogPanel className="bg-white p-6 rounded-lg shadow-lg w-full max-w-md">
                <DialogTitle className="text-xl font-semibold">
                  Nova Transação
                </DialogTitle>

                <form
                  onSubmit={handleCreateTransaction}
                  className="space-y-4 mt-4"
                >
                  <div>
                    <label className="block text-sm font-medium text-gray-700">
                      Valor
                    </label>
                    <input
                      type="number"
                      value={newTransaction.amount}
                      onChange={(e) =>
                        setNewTransaction({
                          ...newTransaction,
                          amount: tryParseFloat(e.target.value),
                        })
                      }
                      className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-indigo-500 focus:border-indigo-500"
                      required
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700">
                      Descrição
                    </label>
                    <input
                      type="text"
                      value={newTransaction.description}
                      onChange={(e) =>
                        setNewTransaction({
                          ...newTransaction,
                          description: e.target.value,
                        })
                      }
                      className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-indigo-500 focus:border-indigo-500"
                    />
                  </div>

                  {/* Combobox - Carteira de Destino */}
                  <div>
                    <label className="block text-sm font-medium text-gray-700">
                      Carteira de Destino
                    </label>
                    <select
                      value={newTransaction.destinationWalletId}
                      onChange={(e) =>
                        setNewTransaction({
                          ...newTransaction,
                          destinationWalletId: e.target.value,
                        })
                      }
                      className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-indigo-500 focus:border-indigo-500"
                      required
                    >
                      <option value="">Selecione uma carteira</option>
                      {wallets &&
                        wallets.length > 0 &&
                        wallets.map((wallet) => (
                          <option key={wallet.id} value={wallet.id}>
                            {wallet.name}
                          </option>
                        ))}
                    </select>
                  </div>

                  <button
                    type="submit"
                    className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors duration-200"
                  >
                    Criar
                  </button>
                </form>
              </DialogPanel>
            </div>
          </Dialog>
        </Transition>

        {/* Lista de Transações */}
        <div className="grid gap-4">
          {loading ? (
            Array(3)
              .fill(0)
              .map((_, index) => (
                <div key={index} className="p-4 bg-white rounded-lg shadow-md">
                  <Skeleton height={20} width="60%" />
                  <Skeleton height={16} width="40%" />
                  <Skeleton height={16} width="80%" />
                </div>
              ))
          ) : transactions.length > 0 ? (
            transactions.map((transaction) => (
              <div
                key={transaction.id}
                className="p-4 bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200"
              >
                <p className="text-lg font-semibold text-gray-800">
                  Valor: R$ {transaction.amount.toFixed(2)}
                </p>
                <p className="text-gray-600">
                  <span className="font-medium">
                    <b>Descrição:</b>
                  </span>{" "}
                  {transaction.description || "N/A"}
                </p>
                <p className="text-gray-600">
                  <span className="font-medium">
                    <b>Tipo:</b>
                  </span>{" "}
                  <span
                    className={
                      transaction.transactionType === "Credit"
                        ? "text-green-600 font-medium"
                        : "text-red-600 font-medium"
                    }
                  >
                    <b>
                      {transaction.transactionType === "Credit"
                        ? "Crédito"
                        : "Débito"}
                    </b>
                  </span>
                </p>
                <p className="text-gray-600">
                  <span className="font-medium">
                    <b>Data/Hora:</b>
                  </span>{" "}
                  {new Date(transaction.date).toLocaleString("pt-BR")}
                </p>
                <p className="text-gray-600">
                  <span className="font-medium">
                    <b>Origem:</b>
                  </span>{" "}
                  {transaction.sourceWallet.name}
                </p>
                <p className="text-gray-600">
                  <span className="font-medium">
                    <b>Destino:</b>
                  </span>{" "}
                  {transaction.destinationWallet.name}
                </p>
              </div>
            ))
          ) : (
            <p className="text-gray-500 text-center py-4">
              Nenhuma transação encontrada.
            </p>
          )}
        </div>

        {/* Controles de Paginação */}
        {transactions.length > 0 && (
          <div className="flex justify-between items-center mt-6">
            <button
              onClick={() => handlePageChange("prev")}
              disabled={pagination.skip === 0}
              className="bg-indigo-600 text-white px-4 py-2 rounded-lg disabled:bg-gray-400 hover:bg-indigo-700 transition-colors duration-200"
            >
              <FaArrowLeft />
            </button>
            <span className="text-gray-700">
              Página {Math.floor(pagination.skip / pagination.take) + 1} de{" "}
              {Math.ceil(total / pagination.take)}
            </span>
            <button
              onClick={() => handlePageChange("next")}
              disabled={pagination.skip + pagination.take >= total}
              className="bg-indigo-600 text-white px-4 py-2 rounded-lg disabled:bg-gray-400 hover:bg-indigo-700 transition-colors duration-200"
            >
              <FaArrowRight />
            </button>
          </div>
        )}
        <div className="min-h-screen bg-gray-100">
          <Toaster position="top-right" reverseOrder={false} />
        </div>
      </div>
    </>
  );
}
