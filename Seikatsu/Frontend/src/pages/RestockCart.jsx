import { useEffect, useMemo, useState } from "react";
import StoreLayout from "../layouts/StoreLayout";
import api from "../Utils/api";

import {
  Trash2,
  ArrowLeft,
  Repeat,
  Package,
  Clock,
  PauseCircle,
  PlayCircle,
  SkipForward,
  ShoppingBag,
  Wallet,
  CalendarDays,
  Activity,
} from "lucide-react";

import { motion } from "framer-motion";
import { useNavigate } from "react-router-dom";

import patternBg from "../assets/restock_pattern.jpg";

const frequencyMap = {
  0: "Daily",
  1: "Weekly",
  2: "BiWeekly",
  3: "Monthly",
  4: "Quarterly",
};

const reverseFrequencyMap = {
  Daily: 0,
  Weekly: 1,
  BiWeekly: 2,
  Monthly: 3,
  Quarterly: 4,
};

const statusMap = {
  0: "Live",
  1: "Paused",
  2: "Skip",
};

const statusReverseMap = {
  Live: 0,
  Paused: 1,
  Skip: 2,
};

export default function RestockCart() {
  const navigate = useNavigate();

  const [cart, setCart] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchRestockCart();
  }, []);

  const fetchRestockCart = async () => {
    try {
      const res = await api.post("/RestockCart/getrestockcart");
      setCart(res.data.data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const updateItem = async (
    itemId,
    quantity,
    frequency
  ) => {
    try {
      await api.patch(
        "/RestockCart/updaterestockcart",
        {
          RestockCartItemId: itemId,
          Quantity: quantity,
          Frequency: frequency,
        }
      );

      fetchRestockCart();
    } catch (err) {
      console.error(err);
    }
  };

  const updateStatus = async (itemId, status) => {
    try {
      await api.patch(
        "/RestockCart/updaterestockstatus",
        {
          RestockCartItemId: itemId,
          Status: status,
        }
      );

      fetchRestockCart();
    } catch (err) {
      console.error(err);
    }
  };

  const deleteItem = async (itemId) => {
    try {
      await api.delete(`/RestockCart/items/${itemId}`);
      fetchRestockCart();
    } catch (err) {
      console.error(err);
    }
  };

  const clearCart = async () => {
    if (!confirm("Clear all items?")) return;

    try {
      await api.delete("/RestockCart/clearrestockcart");
      fetchRestockCart();
    } catch (err) {
      console.error(err);
    }
  };

  const activeItems =
    cart?.items?.filter(
      (i) => statusMap[i.status] !== "Paused"
    ) || [];

  const pausedItems =
    cart?.items?.filter(
      (i) => statusMap[i.status] === "Paused"
    ) || [];

  const upcomingOrders = useMemo(() => {
    if (!cart?.items) return [];

    return [...cart.items]
      .filter((i) => i.nextOrderDate)
      .sort(
        (a, b) =>
          new Date(a.nextOrderDate) -
          new Date(b.nextOrderDate)
      )
      .slice(0, 5);
  }, [cart]);

  const estimatedMonthlySpend =
    cart?.items?.reduce((total, item) => {
      const multiplier =
        item.frequency === 0
          ? 30
          : item.frequency === 1
          ? 4
          : item.frequency === 2
          ? 2
          : 1;

      return (
        total +
        item.productPrice *
          item.quantity *
          multiplier
      );
    }, 0);

  const mostFrequentItem = useMemo(() => {
    if (!cart?.items?.length) return null;

    return [...cart.items].sort(
      (a, b) => a.frequency - b.frequency
    )[0];
  }, [cart]);

  return (
    <StoreLayout>
      <div
        className="min-h-screen pt-24 pb-32"
        style={{
          backgroundImage: `url(${patternBg})`,
          backgroundRepeat: "repeat",
          backgroundSize: "260px",
          backgroundColor: "#f5f7fb",
          fontFamily: "'Plus Jakarta Sans', sans-serif",
        }}
      >
        <div className="max-w-7xl mx-auto px-6 mb-8 flex justify-between">
          <button
            onClick={() => navigate(-1)}
            className="flex items-center gap-2 bg-black text-white px-5 py-3 rounded-xl shadow-lg hover:scale-105 transition"
          >
            <ArrowLeft size={18} />
            Go Back
          </button>
        </div>

        <div className="flex justify-center">
          <motion.div
            initial={{ opacity: 0, y: 40 }}
            animate={{ opacity: 1, y: 0 }}
            className="bg-white w-[1450px] rounded-[32px] shadow-2xl px-14 py-14"
          >
            {/* Header */}
            <div className="flex justify-between items-center mb-12">
              <div>
                <h1 className="text-5xl font-bold text-gray-900">
                  Smart Restock
                </h1>

                <p className="mt-3 text-gray-500 text-lg">
                  Automate your recurring grocery &
                  lifestyle purchases
                </p>
              </div>

              <div className="flex gap-4">
                <button
                  onClick={() => navigate("/home")}
                  className="bg-black text-white px-6 py-3 rounded-xl hover:opacity-90 transition"
                >
                  Browse Products
                </button>

                <button
                  onClick={clearCart}
                  className="border border-red-300 text-red-500 px-6 py-3 rounded-xl hover:bg-red-50 transition"
                >
                  Clear Cart
                </button>
              </div>
            </div>

            {/* Analytics */}
            <div className="grid grid-cols-5 gap-6 mb-14">
              <AnalyticsCard
                title="Active Plans"
                value={activeItems.length}
                icon={<Activity size={20} />}
              />

              <AnalyticsCard
                title="Paused Plans"
                value={pausedItems.length}
                icon={<PauseCircle size={20} />}
              />

              <AnalyticsCard
                title="Monthly Spend"
                value={`₹${estimatedMonthlySpend || 0}`}
                icon={<Wallet size={20} />}
              />

              <AnalyticsCard
                title="Upcoming Orders"
                value={upcomingOrders.length}
                icon={<CalendarDays size={20} />}
              />

              <AnalyticsCard
                title="Top Frequency"
                value={
                  mostFrequentItem
                    ? frequencyMap[
                        mostFrequentItem.frequency
                      ]
                    : "—"
                }
                icon={<Repeat size={20} />}
              />
            </div>

            {/* Upcoming Deliveries */}
            <div className="bg-gradient-to-r from-gray-900 to-black text-white rounded-3xl p-8 mb-12">
              <div className="flex items-center gap-3 mb-6">
                <ShoppingBag />
                <h2 className="text-2xl font-bold">
                  Upcoming Deliveries
                </h2>
              </div>

              <div className="space-y-4">
                {upcomingOrders.length === 0 ? (
                  <p className="text-gray-400">
                    No upcoming deliveries
                  </p>
                ) : (
                  upcomingOrders.map((item) => (
                    <div
                      key={item.id}
                      className="flex justify-between items-center bg-white/10 rounded-2xl px-5 py-4"
                    >
                      <div className="flex items-center gap-4">
                        <img
                          src={item.productImageUrl}
                          alt={item.productName}
                          className="w-14 h-14 rounded-xl object-cover"
                        />

                        <div>
                          <p className="font-semibold">
                            {item.productName}
                          </p>

                          <p className="text-sm text-gray-300">
                            {
                              frequencyMap[
                                item.frequency
                              ]
                            }
                          </p>
                        </div>
                      </div>

                      <div className="text-right">
                        <p className="font-semibold">
                          {new Date(
                            item.nextOrderDate
                          ).toDateString()}
                        </p>

                        <p className="text-sm text-gray-300">
                          Qty: {item.quantity}
                        </p>
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>

            {/* Loading */}
            {loading ? (
              <p className="text-gray-400">
                Loading subscriptions...
              </p>
            ) : cart?.items?.length === 0 ? (
              <EmptyState />
            ) : (
              <>
                {/* Active */}
                <SectionTitle title="Active Subscriptions" />

                <div className="space-y-6 mb-14">
                  {activeItems.map((item) => (
                    <RestockItemRow
                      key={item.id}
                      item={item}
                      updateItem={updateItem}
                      deleteItem={deleteItem}
                      updateStatus={updateStatus}
                    />
                  ))}
                </div>

                {/* Paused */}
                {pausedItems.length > 0 && (
                  <>
                    <SectionTitle title="Paused Subscriptions" />

                    <div className="space-y-6">
                      {pausedItems.map((item) => (
                        <RestockItemRow
                          key={item.id}
                          item={item}
                          updateItem={updateItem}
                          deleteItem={deleteItem}
                          updateStatus={updateStatus}
                        />
                      ))}
                    </div>
                  </>
                )}
              </>
            )}
          </motion.div>
        </div>
      </div>
    </StoreLayout>
  );
}

function AnalyticsCard({
  title,
  value,
  icon,
}) {
  return (
    <div className="bg-gray-50 rounded-3xl p-6 shadow-sm hover:shadow-lg transition">
      <div className="flex justify-between items-center mb-4">
        <p className="text-gray-500 text-sm">
          {title}
        </p>

        <div className="bg-black text-white p-2 rounded-xl">
          {icon}
        </div>
      </div>

      <h2 className="text-3xl font-bold text-gray-900">
        {value}
      </h2>
    </div>
  );
}

function SectionTitle({ title }) {
  return (
    <div className="mb-6">
      <h2 className="text-3xl font-bold text-gray-900">
        {title}
      </h2>
    </div>
  );
}

function EmptyState() {
  return (
    <div className="flex flex-col items-center py-28 text-gray-400">
      <Package size={60} />

      <p className="mt-6 text-2xl font-semibold">
        Your restock cart is empty
      </p>

      <p className="text-sm mt-2">
        Add products and automate recurring
        purchases
      </p>
    </div>
  );
}

function RestockItemRow({
  item,
  updateItem,
  deleteItem,
  updateStatus,
}) {
  const status =
    statusMap[item.status] || "Live";

  return (
    <div className="border border-gray-200 rounded-[28px] p-8 hover:shadow-xl transition bg-white">
      <div className="flex justify-between">
        {/* LEFT */}
        <div className="flex gap-6">
          <img
            src={item.productImageUrl}
            alt={item.productName}
            className="w-28 h-28 rounded-2xl object-cover shadow-md"
          />

          <div>
            <div className="flex items-center gap-3">
              <h2 className="text-2xl font-bold text-gray-900">
                {item.productName}
              </h2>

              <StatusBadge status={status} />
            </div>

            <p className="text-gray-500 mt-2">
              ₹{item.productPrice} ×{" "}
              {item.quantity}
            </p>

            <div className="flex gap-6 mt-4 text-sm text-gray-500">
              <span className="flex items-center gap-2">
                <Repeat size={15} />
                {
                  frequencyMap[item.frequency]
                }
              </span>

              <span className="flex items-center gap-2">
                <Clock size={15} />
                {item.nextOrderDate
                  ? new Date(
                      item.nextOrderDate
                    ).toDateString()
                  : "Paused"}
              </span>
            </div>

            {item.lastOrderedAt && (
              <p className="text-xs text-gray-400 mt-3">
                Last ordered on{" "}
                {new Date(
                  item.lastOrderedAt
                ).toDateString()}
              </p>
            )}
          </div>
        </div>

        {/* RIGHT */}
        <div className="flex flex-col items-end justify-between">
          <div className="flex gap-3">
            {/* quantity */}
            <input
              type="number"
              min="1"
              value={item.quantity}
              onChange={(e) =>
                updateItem(
                  item.id,
                  Number(e.target.value),
                  item.frequency
                )
              }
              className="w-20 border rounded-xl px-3 py-2"
            />

            {/* frequency */}
            <select
              value={
                frequencyMap[item.frequency]
              }
              onChange={(e) =>
                updateItem(
                  item.id,
                  item.quantity,
                  reverseFrequencyMap[
                    e.target.value
                  ]
                )
              }
              className="border rounded-xl px-4 py-2"
            >
              <option>Daily</option>
              <option>Weekly</option>
              <option>BiWeekly</option>
              <option>Monthly</option>
              <option>Quarterly</option>
            </select>
          </div>

          {/* actions */}
          <div className="flex gap-3 mt-6">
            {status === "Paused" ? (
              <button
                onClick={() =>
                  updateStatus(item.id, 0)
                }
                className="flex items-center gap-2 bg-green-100 text-green-700 px-4 py-2 rounded-xl"
              >
                <PlayCircle size={16} />
                Resume
              </button>
            ) : (
              <button
                onClick={() =>
                  updateStatus(item.id, 1)
                }
                className="flex items-center gap-2 bg-yellow-100 text-yellow-700 px-4 py-2 rounded-xl"
              >
                <PauseCircle size={16} />
                Pause
              </button>
            )}

            <button
              onClick={() =>
                updateStatus(item.id, 2)
              }
              className="flex items-center gap-2 bg-blue-100 text-blue-700 px-4 py-2 rounded-xl"
            >
              <SkipForward size={16} />
              Skip
            </button>

            <button
              onClick={() => deleteItem(item.id)}
              className="bg-red-100 text-red-600 p-3 rounded-xl hover:bg-red-200"
            >
              <Trash2 size={18} />
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

function StatusBadge({ status }) {
  const styles = {
    Live: "bg-green-100 text-green-700",
    Paused: "bg-yellow-100 text-yellow-700",
    Skip: "bg-blue-100 text-blue-700",
  };

  return (
    <span
      className={`px-3 py-1 rounded-full text-xs font-semibold ${styles[status]}`}
    >
      {status}
    </span>
  );
}