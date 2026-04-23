import { useEffect, useState } from "react";
import StoreLayout from "../layouts/StoreLayout";
import api from "../Utils/api";

import {
    Trash2,
    ArrowLeft,
    Repeat,
    Package,
    Clock,
    Plus
} from "lucide-react";

import { motion } from "framer-motion";
import { useNavigate } from "react-router-dom";

import patternBg from "../assets/restock_pattern.jpg";


// mapping
const frequencyMap = {
    0: "Daily",
    1: "Weekly",
    2: "BiWeekly",
    3: "Monthly",
    4: "Quarterly"

};

// reverse
const reverseFrequencyMap = {
    Daily: 0,
    Weekly: 1,
    BiWeekly: 2,
    Monthly: 3,
    Quarterly: 4
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

            const res =
                await api.post("/RestockCart/getrestockcart");

            setCart(res.data.data);

        } catch (err) {

            console.error(err);

        } finally {

            setLoading(false);

        }

    };


    // update
    const updateItem = async (
        itemId,
        quantity,
        frequency
    ) => {

        await api.patch(
            "/RestockCart/updaterestockcart",
            {
                RestockCartItemId: itemId,
                Quantity: quantity,
                Frequency:frequency
            }
        );

        fetchRestockCart();

    };


    // delete
    const deleteItem = async (itemId) => {

        await api.delete(
            `/RestockCart/items/${itemId}`
        );

        fetchRestockCart();

    };


    // clear
    const clearCart = async () => {

        if (!confirm("Clear all items?")) return;

        await api.delete(
            "/RestockCart/clearrestockcart"
        );

        fetchRestockCart();

    };


    // next-date
    const nextUpcomingDate =
        cart?.items?.length
            ? cart.items
                .map(i => new Date(i.nextOrderDate))
                .sort((a, b) => a - b)[0]
            : null;


    // estimate
    const estimatedMonthlySpend =
        cart?.items?.reduce((total, item) => {

            const multiplier =
                item.frequency === 0 ? 4 :
                    item.frequency === 1 ? 2 :
                        1;

            return total +
                (item.productPrice *
                    item.quantity *
                    multiplier);

        }, 0);


    return (

        <StoreLayout>

            <div
                className="min-h-screen pt-24 pb-32"
                style={{
                    backgroundImage: `url(${patternBg})`,
                    backgroundRepeat: "repeat",
                    backgroundSize: "260px",
                    backgroundColor: "#f1f5f9"
                }}
            >


                <div className="max-w-7xl mx-auto px-6 mb-8 flex justify-between">

                    <button
                        onClick={() => navigate(-1)}
                        className="flex items-center gap-2 bg-black text-white px-5 py-2 rounded-lg shadow hover:bg-gray-900 transition"
                    >
                        <ArrowLeft size={18} />
                        Go Back
                    </button>

                </div>


                <div className="flex justify-center">

                    <motion.div
                        initial={{ opacity: 0, y: 40 }}
                        animate={{ opacity: 1, y: 0 }}
                        className="bg-white w-[1300px] rounded-3xl shadow-xl px-16 py-14"
                        style={{ fontFamily: "Plus Jakarta Sans" }}
                    >


                        {/* header */}

                        <div className="mb-10 flex justify-between items-center">

                            <div>

                                <h1 className="text-4xl font-semibold text-gray-800">
                                    Restock Cart
                                </h1>

                                {cart?.items?.length > 0 && (

                                    <p className="text-gray-500 mt-2 flex items-center gap-2">
                                        <Repeat size={16} />
                                        Auto-restock active for
                                        <b>{cart.totalItemsCount}</b>
                                        items
                                    </p>

                                )}

                            </div>


                            {cart?.items?.length === 0 && (

                                <button
                                    onClick={() => navigate("/home")}
                                    className="flex items-center gap-2 border border-gray-300 px-5 py-2 rounded-lg hover:bg-gray-100 transition"
                                >
                                    <Plus size={18} />
                                    Browse Products
                                </button>

                            )}

                        </div>


                        {/* summary */}

                        {cart && (

                            <div className="grid grid-cols-4 gap-6 mb-10">

                                <SummaryCard
                                    title="Items"
                                    value={cart.totalItemsCount}
                                />

                                <SummaryCard
                                    title="Cart Total"
                                    value={`₹${cart.totalPrice}`}
                                />

                                <SummaryCard
                                    title="Next Delivery"
                                    value={
                                        nextUpcomingDate
                                            ? nextUpcomingDate.toDateString()
                                            : "—"
                                    }
                                />

                                <SummaryCard
                                    title="Monthly Estimate"
                                    value={`₹${estimatedMonthlySpend || 0}`}
                                />

                            </div>

                        )}


                        {/* content */}

                        {loading ? (

                            <p className="text-gray-400">
                                Loading restock items...
                            </p>

                        ) : cart?.items?.length === 0 ? (

                            <EmptyState />

                        ) : (

                            <>

                                <div className="space-y-6 max-h-[520px] overflow-y-auto pr-2">

                                    {cart.items.map(item => (

                                        <RestockItemRow
                                            key={item.id}
                                            item={item}
                                            updateItem={updateItem}
                                            deleteItem={deleteItem}
                                        />

                                    ))}

                                </div>


                                <button
                                    onClick={clearCart}
                                    className="mt-12 text-sm text-red-500 hover:underline"
                                >
                                    Clear restock cart
                                </button>

                            </>

                        )}

                    </motion.div>

                </div>

            </div>

        </StoreLayout>

    );

}


// card
function SummaryCard({ title, value }) {

    return (

        <div className="bg-gray-50 rounded-xl p-5">

            <p className="text-sm text-gray-500">
                {title}
            </p>

            <p className="text-2xl font-semibold">
                {value}
            </p>

        </div>

    );

}


// empty
function EmptyState() {

    return (

        <div className="flex flex-col items-center py-20 text-gray-400">

            <Package size={48} />

            <p className="mt-4 text-lg">
                Your restock cart is empty
            </p>

            <p className="text-sm">
                Add items to automate repeat purchases 🛒
            </p>

        </div>

    );

}


// row
function RestockItemRow({
    item,
    updateItem,
    deleteItem
}) {

    return (

        <div className="flex justify-between items-center border rounded-xl px-6 py-4 hover:bg-gray-50 transition shadow-sm">


            {/* info */}

            <div className="flex items-center gap-6">

                <img
                    src={item.productImageUrl}
                    alt={item.productName}
                    className="w-20 h-20 rounded-xl object-cover shadow"
                />


                <div>

                    <p className="text-xl font-medium text-gray-800">
                        {item.productName}
                    </p>


                    <p className="text-sm text-gray-400 flex items-center gap-2">
                        <Clock size={14} />
                        Next order:
                        {new Date(item.nextOrderDate).toDateString()}
                    </p>


                    {item.lastOrderedAt && (

                        <p className="text-xs text-gray-400">
                            Last ordered:
                            {new Date(item.lastOrderedAt).toDateString()}
                        </p>

                    )}


                    <span className="text-xs bg-purple-100 text-purple-700 px-2 py-1 rounded mt-1 inline-block">
                        {frequencyMap[item.frequency]}
                    </span>

                </div>

            </div>


            {/* controls */}

            <div className="flex items-center gap-4">


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
                    className="w-16 border rounded px-2 py-1"
                />


                <select
                    value={frequencyMap[item.frequency]}
                    onChange={(e) =>
                        updateItem(
                            item.id,
                            item.quantity,
                            reverseFrequencyMap[e.target.value]
                        )
                    }
                    className="border rounded px-2 py-1 text-sm"
                >
                    <option>Daily</option>
                    <option>Weekly</option>
                    <option>BiWeekly</option>
                    <option>Monthly</option>
                    <option>Quarterly</option>

                </select>


                <Trash2
                    size={20}
                    onClick={() => deleteItem(item.id)}
                    className="text-gray-400 hover:text-red-500 cursor-pointer"
                />

            </div>

        </div>

    );

}