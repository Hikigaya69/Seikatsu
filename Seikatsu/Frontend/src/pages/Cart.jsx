import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../Utils/api";
import StoreLayout from "../layouts/StoreLayout";
import { ArrowLeft } from "lucide-react";

export default function Cart() {
    const [cart, setCart] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        fetchCart();
    }, []);

    const fetchCart = async () => {
        try {
            const res = await api.get("/Cart/getcart");
            setCart(res.data.data);
        } catch (err) {
            console.error(err);
        }
    };

    const clearCart = async () => {
        try {
            await api.delete("/Cart/clearcart");
            fetchCart();
        } catch (err) {
            console.error(err);
        }
    };

    const updateQuantity = async (cartItemId, quantity) => {
        if (quantity < 1) return;
        try {
            await api.patch("/Cart/updatecart", { cartItemId, quantity });
            fetchCart();
        } catch (err) {
            console.error(err);
        }
    };

    const removeItem = async (cartItemId) => {
        try {
            await api.delete(`/Cart/items/${cartItemId}`);
            fetchCart();
        } catch (err) {
            console.error(err);
        }
    };

    if (!cart || cart.items.length === 0) {
        return (
            <StoreLayout>
                <div className="max-w-7xl mx-auto px-6 pt-6">
                    <button
                        onClick={() => navigate(-1)}
                        className="flex items-center gap-2 border border-gray-300 px-4 py-2 rounded-lg shadow-sm hover:shadow-md hover:bg-gray-50 transition"
                    >
                        <ArrowLeft size={18} />
                        Go Back
                    </button>
                </div>
                <div className="flex flex-col items-center justify-center py-24">
                    <h2 className="text-2xl font-semibold mb-4">Your cart is empty 🛒</h2>
                    <p className="text-gray-500 mb-6">Looks like you haven't added anything yet.</p>
                    <button
                        onClick={() => navigate("/home")}
                        className="bg-[#284b63] text-white px-6 py-3 rounded-lg"
                    >
                        Continue Shopping
                    </button>
                </div>
            </StoreLayout>
        );
    }

    return (
        <StoreLayout>
            <div className="max-w-7xl mx-auto px-6 pt-6">
                <button
                    onClick={() => navigate(-1)}
                    className="flex items-center gap-2 border border-gray-300 px-4 py-2 rounded-lg shadow-sm hover:shadow-md hover:bg-gray-50 transition"
                >
                    <ArrowLeft size={18} />
                    Go Back
                </button>
            </div>

            <div className="max-w-6xl mx-auto px-6 py-10">
                <div className="bg-[#3c6e71] text-white rounded-lg py-3 text-center font-semibold text-xl mb-8">
                    Your Cart ({cart.totalItems} items)
                </div>

                <div className="grid grid-cols-3 gap-8">

                    {/* ITEMS */}
                    <div className="col-span-2 space-y-6">
                        {cart.items.map((item) => (
                            <div
                                key={item.cartItemId}
                                className="flex justify-between items-center bg-white shadow rounded-lg p-4"
                            >
                                <div className="flex gap-4 items-center">
                                    <img
                                        src={item.productImageUrl || "/ramen.jpg"}
                                        className="w-24 h-24 object-cover rounded"
                                    />
                                    <div>
                                        <h3 className="font-semibold">{item.productName}</h3>
                                        <p className="text-gray-500">¥{item.price}</p>
                                    </div>
                                </div>

                                <div className="flex items-center gap-6">
                                    <div className="flex items-center border rounded-lg overflow-hidden">
                                        <button
                                            onClick={() => updateQuantity(item.cartItemId, item.quantity - 1)}
                                            className="px-4 py-2 bg-gray-200 hover:bg-gray-300"
                                        >
                                            −
                                        </button>
                                        <span className="px-6">{item.quantity}</span>
                                        <button
                                            onClick={() => updateQuantity(item.cartItemId, item.quantity + 1)}
                                            className="px-4 py-2 bg-gray-200 hover:bg-gray-300"
                                        >
                                            +
                                        </button>
                                    </div>

                                    <span className="font-bold">¥{item.itemTotal}</span>

                                    <button
                                        onClick={() => removeItem(item.cartItemId)}
                                        className="text-red-500 hover:text-red-700"
                                    >
                                        Remove
                                    </button>
                                </div>
                            </div>
                        ))}
                    </div>

                    {/* SUMMARY */}
                    <div className="bg-white shadow rounded-lg p-6 h-fit">
                        <h2 className="text-lg font-semibold mb-4">Cart Summary</h2>

                        <div className="flex justify-between mb-2">
                            <span>Total Price</span>
                            <span className="font-bold">¥{cart.totalPrice}</span>
                        </div>

                        <button
                            onClick={clearCart}
                            className="mt-2 w-full border border-red-400 text-red-500 py-2 rounded-lg hover:bg-red-50"
                        >
                            Clear Cart
                        </button>

                        <button
                            onClick={() => navigate("/checkout")}
                            className="mt-4 w-full bg-[#284b63] text-white py-3 rounded-lg"
                        >
                            Checkout
                        </button>
                    </div>

                </div>
            </div>
        </StoreLayout>
    );
}