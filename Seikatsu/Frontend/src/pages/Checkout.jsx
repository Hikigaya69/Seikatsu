import { useEffect, useState } from "react";
import axios from "axios";
import StoreLayout from "../layouts/StoreLayout";
import { useNavigate } from "react-router-dom";
import { ArrowLeft, MapPin, CheckCircle } from "lucide-react";


const loadRazorpayScript = () => {
    return new Promise((resolve) => {
        if (document.querySelector('script[src="https://checkout.razorpay.com/v1/checkout.js"]')) {
            resolve(true);
            return;
        }
        const script = document.createElement("script");
        script.src = "https://checkout.razorpay.com/v1/checkout.js";
        script.onload = () => resolve(true);
        script.onerror = () => resolve(false);
        document.body.appendChild(script);
    });
};

export default function Checkout() {
    const [cart, setCart] = useState(null);
    const [addresses, setAddresses] = useState([]);
    const [selectedAddressId, setSelectedAddressId] = useState(null);
    const [showAddresses, setShowAddresses] = useState(false);
    const [paying, setPaying] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        fetchCart();
        fetchAddresses();
    }, []);

    const fetchCart = async () => {
        try {
            const res = await axios.post(
                `/api/Cart/getcart`,
                {},
                { withCredentials: true }
            );
            setCart(res.data.data);
        } catch (err) {
            console.error(err);
        }
    };

    const fetchAddresses = async () => {
        try {
            const res = await axios.get(
                `/api/Address/getalladdress`,
                { withCredentials: true }
            );
            const data = res.data.data;
            setAddresses(data);
            // auto-select default address
            const defaultAddr = data.find((a) => a.isDefault);
            if (defaultAddr) setSelectedAddressId(defaultAddr.id);
        } catch (err) {
            console.error(err);
        }
    };

    const selectedAddress = addresses.find((a) => a.id === selectedAddressId);

    // mirror backend calculation exactly.. can be called using create order-api but the problem is the current logic is bit messy so needs refactor
    const subTotal = cart?.items?.reduce((sum, i) => sum + i.itemTotal, 0) ?? 0;
    const deliveryCharge = subTotal > 500 ? 0 : 49;
    const tax = Math.round(subTotal * 0.18 * 100) / 100;
    const grandTotal = subTotal + deliveryCharge + tax;

    const handlePayment = async () => {
        if (!selectedAddressId) {
            alert("Please select a delivery address.");
            return;
        }

        setPaying(true);

        try {
            //  load razorpay script
            const scriptLoaded = await loadRazorpayScript();
            if (!scriptLoaded) {
                alert("Failed to load Razorpay. Check your internet connection.");
                setPaying(false);
                return;
            }

            // create order on backend
            const orderRes = await axios.post(
                `/api/Order/initiateorder`,
                {
                    CartId: cart.cartid,
                    AddressId: selectedAddressId,
                },
                { withCredentials: true }
            );

            const { orderId, razorpayOrderId, amount, currency, keyId } =
                orderRes.data.data;

            // open razorpay popup
            const options = {
                key: keyId,
                amount: amount * 100, // paise but the 
                currency: currency,
                name: "Seikatsu",
                description: "Order Payment",
                order_id: razorpayOrderId,

                //  on success — verify with backend
                handler: async function (response) {
                    try {
                        const verifyRes = await axios.post(
                            `/api/Order/verifyorder`,
                            {
                                razorpayOrderId: response.razorpay_order_id,
                                razorpayPaymentId: response.razorpay_payment_id,
                                razorpaySignature: response.razorpay_signature,
                            },
                            { withCredentials: true }
                        );

                        if (verifyRes.status === 200) {
                            navigate(`/order-success/${orderId}`); // this page needs to be created
                        } else {
                            alert("Payment verification failed. Please contact support.");
                        }
                    } catch (err) {
                        console.error(err);
                        alert("Payment verification failed.");
                    } finally {
                        setPaying(false);
                    }
                },

                prefill: {
                    name: selectedAddress?.fullName ?? "",
                    contact: selectedAddress?.phoneNumber ?? "",
                },

                theme: { color: "#284b63" },

                modal: {
                    ondismiss: function () {
                        setPaying(false);
                    },
                },
            };

            const rzp = new window.Razorpay(options);

            rzp.on("payment.failed", function (response) {
                console.error("Payment failed:", response.error);
                alert(`Payment failed: ${response.error.description}`);
                setPaying(false);
            });

            rzp.open();

        } catch (err) {
            console.error(err);
            alert("Something went wrong while creating the order.");
            setPaying(false);
        }
    }; //god knows about this part

    if (!cart) {
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
                <div className="p-10 text-center text-gray-500">Loading checkout...</div>
            </StoreLayout>
        );
    }

    return (
        <StoreLayout>
            {/* back button */}
            <div className="max-w-7xl mx-auto px-6 pt-6">
                <button
                    onClick={() => navigate(-1)}
                    className="flex items-center gap-2 border border-gray-300 px-4 py-2 rounded-lg shadow-sm hover:shadow-md hover:bg-gray-50 transition"
                >
                    <ArrowLeft size={18} />
                    Go Back
                </button>
            </div>

            <div className="max-w-7xl mx-auto px-6 py-10">

                {/* header */}
                <div className="bg-[#3c6e71] text-white rounded-lg py-3 text-center font-semibold text-xl mb-8">
                    Checkout ({cart.items.length} items)
                </div>

                <div className="grid grid-cols-3 gap-8">

                    {/* ── LEFT SIDE ── */}
                    <div className="col-span-2 space-y-6">

                        {/* ADDRESS SECTION */}
                        <div className="bg-white rounded-xl shadow p-6">
                            <div className="flex justify-between items-center">
                                <h2 className="font-semibold text-lg">Delivering to</h2>
                                <button
                                    onClick={() => setShowAddresses(!showAddresses)}
                                    className="text-[#284b63] text-sm font-medium hover:underline"
                                >
                                    {showAddresses ? "Cancel" : "Change"}
                                </button>
                            </div>

                            {/* selected address display */}
                            {selectedAddress && !showAddresses && (
                                <div className="mt-4 flex gap-3 text-gray-700">
                                    <MapPin size={18} className="mt-1 text-[#284b63] shrink-0" />
                                    <div>
                                        <p className="font-medium">{selectedAddress.fullName}</p>
                                        <p className="text-sm">{selectedAddress.addressLine1}{selectedAddress.addressLine2 ? `, ${selectedAddress.addressLine2}` : ""}</p>
                                        <p className="text-sm">{selectedAddress.city} — {selectedAddress.postalCode}</p>
                                        <p className="text-sm">{selectedAddress.country}</p>
                                        <p className="text-sm text-gray-500 mt-1">{selectedAddress.phoneNumber}</p>
                                    </div>
                                </div>
                            )}

                            {/* no address selected */}
                            {!selectedAddress && !showAddresses && (
                                <p className="text-gray-500 mt-4 text-sm">
                                    No address selected. Click <span className="text-[#284b63] font-medium">Change</span> to select one.
                                </p>
                            )}

                            {/* address picker list */}
                            {showAddresses && (
                                <div className="mt-4 space-y-3">
                                    {addresses.length === 0 ? (
                                        <p className="text-sm text-gray-500">
                                            No saved addresses. Please add one in your profile.
                                        </p>
                                    ) : (
                                        addresses.map((addr) => (
                                            <div
                                                key={addr.id}
                                                onClick={() => {
                                                    setSelectedAddressId(addr.id);
                                                    setShowAddresses(false);
                                                }}
                                                className={`border rounded-lg p-4 cursor-pointer transition ${selectedAddressId === addr.id
                                                        ? "border-[#284b63] bg-blue-50"
                                                        : "hover:border-gray-400"
                                                    }`}
                                            >
                                                <div className="flex justify-between items-start">
                                                    <div>
                                                        <p className="font-medium">{addr.fullName}</p>
                                                        <p className="text-sm text-gray-600">
                                                            {addr.addressLine1}{addr.addressLine2 ? `, ${addr.addressLine2}` : ""}
                                                        </p>
                                                        <p className="text-sm text-gray-600">
                                                            {addr.city} — {addr.postalCode}, {addr.country}
                                                        </p>
                                                        <p className="text-sm text-gray-500">{addr.phoneNumber}</p>
                                                    </div>
                                                    {selectedAddressId === addr.id && (
                                                        <CheckCircle size={20} className="text-[#284b63] shrink-0" />
                                                    )}
                                                </div>
                                                {addr.isDefault && (
                                                    <span className="text-xs bg-green-100 text-green-700 px-2 py-0.5 rounded-full mt-2 inline-block">
                                                        Default
                                                    </span>
                                                )}
                                            </div>
                                        ))
                                    )}
                                </div>
                            )}
                        </div>

                        {/* PAYMENT METHOD */}
                        <div className="bg-white rounded-xl shadow p-6">
                            <h2 className="font-semibold text-lg mb-4">Payment Method</h2>
                            <div className="border rounded-lg p-4 border-[#284b63] bg-blue-50">
                                <p className="font-medium">Razorpay</p>
                                <p className="text-sm text-gray-500 mt-1">
                                    Credit / Debit Card • UPI • NetBanking • Wallets
                                </p>
                            </div>
                        </div>

                    </div>

                    {/* ── RIGHT SIDE SUMMARY ── */}
                    <div className="bg-white rounded-xl shadow p-6 h-fit sticky top-6">
                        <h2 className="font-semibold text-lg mb-4">Order Summary</h2>

                        {/* item list */}
                        <div className="space-y-3 mb-4">
                            {cart.items.map((item) => (
                                <div key={item.cartItemId} className="flex justify-between text-sm">
                                    <span className="text-gray-700">{item.productName} ×{item.quantity}</span>
                                    <span className="font-medium">¥{item.itemTotal}</span>
                                </div>
                            ))}
                        </div>

                        <hr className="my-4" />

                        {/* bill breakdown */}
                        <div className="space-y-2 text-sm text-gray-600 mb-4">
                            <div className="flex justify-between">
                                <span>Subtotal</span>
                                <span>¥{subTotal.toFixed(2)}</span>
                            </div>
                            <div className="flex justify-between">
                                <span>Delivery</span>
                                <span>
                                    {deliveryCharge === 0
                                        ? <span className="text-green-600 font-medium">Free</span>
                                        : `¥${deliveryCharge}`}
                                </span>
                            </div>
                            <div className="flex justify-between">
                                <span>Tax (18%)</span>
                                <span>¥{tax.toFixed(2)}</span>
                            </div>
                        </div>

                        <hr className="my-4" />

                        <div className="flex justify-between font-bold text-lg mb-6">
                            <span>Total</span>
                            <span>¥{grandTotal.toFixed(2)}</span>
                        </div>

                        <button
                            onClick={handlePayment}
                            disabled={paying || !selectedAddressId}
                            className="w-full bg-[#284b63] text-white py-3 rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:opacity-90 transition font-medium"
                        >
                            {paying ? "Processing..." : "Proceed to Payment"}
                        </button>

                        {!selectedAddressId && (
                            <p className="text-xs text-red-500 text-center mt-2">
                                Please select a delivery address to continue
                            </p>
                        )}

                        {subTotal <= 500 && (
                            <p className="text-xs text-gray-400 text-center mt-3">
                                Add items worth ¥{(5000 - subTotal).toFixed(2)} more for free delivery
                            </p>
                        )}
                    </div>

                </div>
            </div>
        </StoreLayout>
    );
}