import {
  CheckCircle2,
  CalendarDays,
  CreditCard,
  Receipt,
} from "lucide-react";

import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import api from "../Utils/api";
import StoreLayout from "../layouts/StoreLayout";

export default function PaymentSuccess() {

  const { orderId } = useParams();

  const navigate = useNavigate();

  const [order, setOrder] = useState(null);

  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchOrder();
  }, []);

  const fetchOrder = async () => {
    try {

      const res = await api.get(
        `/Order/ordersummarybyOrderId?orderId=${orderId}`,
        { withCredentials: true }
      );

      setOrder(res.data.data);

    } catch (err) {

      console.error(err);

    } finally {

      setLoading(false);

    }
  };

  if (loading) {
    return (
      <StoreLayout>
        <div className="min-h-screen flex items-center justify-center text-lg">
          Loading order details...
        </div>
      </StoreLayout>
    );
  }

  if (!order) {
    return (
      <StoreLayout>
        <div className="min-h-screen flex items-center justify-center text-lg">
          Order not found.
        </div>
      </StoreLayout>
    );
  }

  return (
   

    <StoreLayout>
         <div className="font-jakarta">

      <div className="min-h-screen bg-[#f6f8f7] py-16 px-6">

        <div className="max-w-5xl mx-auto">

          <div className="bg-white rounded-[36px] shadow-xl border p-12">

            {/* SUCCESS ICON */}

            <div className="flex justify-center">

              <div className="relative">

                <div className="w-36 h-36 rounded-full bg-green-100 flex items-center justify-center">

                  <CheckCircle2
                    size={70}
                    className="text-green-600"
                  />

                </div>

                <div className="absolute inset-0 rounded-full animate-ping bg-green-200 opacity-20"></div>

              </div>

            </div>

            {/* TITLE */}

            <div className="text-center mt-10">

              <h1 className="text-5xl font-bold text-[#0f172a]">

                Payment Successful!

              </h1>

              <p className="text-gray-500 text-lg mt-5 leading-relaxed">

                Thank you for your purchase.
                Your order has been placed successfully.

              </p>

            </div>

            {/* SUCCESS STRIP */}

            <div className="mt-10 bg-green-50 border border-green-200 rounded-2xl p-6 flex items-center gap-5">

              <div className="w-14 h-14 rounded-full bg-white flex items-center justify-center shadow">

                <CheckCircle2
                  className="text-green-700"
                  size={28}
                />

              </div>

              <div>

                <h2 className="text-lg font-semibold text-[#111827]">

                  Your order is confirmed

                </h2>

                <p className="text-gray-500 mt-1">

                  We will notify you once your items are shipped.

                </p>

              </div>

            </div>

            {/* ORDER DETAILS */}

            <div className="grid md:grid-cols-3 gap-6 mt-10">

              {/* ORDER ID */}

              <div className="bg-[#f8fafc] rounded-2xl p-5 border">

                <div className="flex items-center gap-3 mb-3">

                  <Receipt
                    size={22}
                    className="text-[#284b63]"
                  />

                  <p className="text-gray-500 text-sm">

                    Order ID

                  </p>

                </div>

                <h3 className="font-semibold text-lg break-all">

                  {orderId}

                </h3>

              </div>

              {/* ORDER DATE */}

              <div className="bg-[#f8fafc] rounded-2xl p-5 border">

                <div className="flex items-center gap-3 mb-3">

                  <CalendarDays
                    size={22}
                    className="text-[#284b63]"
                  />

                  <p className="text-gray-500 text-sm">

                    Order Date

                  </p>

                </div>

                <h3 className="font-semibold text-lg">

                  {new Date().toLocaleDateString()}

                </h3>

              </div>

              {/* PAYMENT METHOD */}

              <div className="bg-[#f8fafc] rounded-2xl p-5 border">

                <div className="flex items-center gap-3 mb-3">

                  <CreditCard
                    size={22}
                    className="text-[#284b63]"
                  />

                  <p className="text-gray-500 text-sm">

                    Payment Method

                  </p>

                </div>

                <h3 className="font-semibold text-lg">

                  Razorpay

                </h3>

              </div>

            </div>

            {/* ORDER ITEMS */}

            <div className="mt-14">

              <h2 className="text-2xl font-bold text-[#111827] mb-6">

                Ordered Items

              </h2>

              <div className="space-y-4">

                {order.items.map((item) => (

                  <div
                    key={item.productId}
                    className="flex items-center justify-between border rounded-2xl p-4 hover:shadow-md transition"
                  >

                    <div className="flex items-center gap-5">

                      <img
                        src={item.productImageUrl}
                        alt={item.productName}
                        className="w-24 h-24 rounded-2xl object-cover border"
                      />

                      <div>

                        <h3 className="font-semibold text-lg text-[#111827]">

                          {item.productName}

                        </h3>

                        <p className="text-gray-500 text-sm mt-1">

                          Quantity: {item.quantity}

                        </p>

                        <p className="text-gray-500 text-sm">

                          ₹{item.unitPrice} each

                        </p>

                      </div>

                    </div>

                    <h2 className="font-bold text-xl text-[#0f172a]">

                      ₹{item.lineTotal}

                    </h2>

                  </div>

                ))}

              </div>

            </div>

            {/* PAYMENT SUMMARY */}

            <div className="mt-14 bg-[#fafafa] border rounded-3xl p-8">

              <h2 className="text-2xl font-bold text-[#111827] mb-6">

                Payment Summary

              </h2>

              <div className="space-y-4 text-gray-700">

                <div className="flex justify-between">

                  <span>Subtotal</span>

                  <span>₹{order.subTotal}</span>

                </div>

                <div className="flex justify-between">

                  <span>Delivery Charge</span>

                  <span>

                    {order.deliveryCharge === 0
                      ? "Free"
                      : `₹${order.deliveryCharge}`}

                  </span>

                </div>

                <div className="flex justify-between">

                  <span>Tax</span>

                  <span>₹{order.tax}</span>

                </div>

                <hr className="my-4" />

                <div className="flex justify-between font-bold text-2xl text-[#0f172a]">

                  <span>Total Paid</span>

                  <span>₹{order.totalAmount}</span>

                </div>

              </div>

            </div>

            {/* DELIVERY ADDRESS */}

            <div className="mt-14 border rounded-3xl p-8">

              <h2 className="text-2xl font-bold text-[#111827] mb-5">

                Delivery Address

              </h2>

              <div className="text-gray-700 leading-8">

                <p className="font-semibold text-lg">

                  {order.deliveryAddress.fullName}

                </p>

                <p>

                  {order.deliveryAddress.addressLine1}

                </p>

                {order.deliveryAddress.addressLine2 && (
                  <p>

                    {order.deliveryAddress.addressLine2}

                  </p>
                )}

                <p>

                  {order.deliveryAddress.city},
                  {" "}
                  {order.deliveryAddress.postalCode}

                </p>

                <p>

                  {order.deliveryAddress.country}

                </p>

                <p className="mt-2 text-gray-500">

                  {order.deliveryAddress.phoneNumber}

                </p>

              </div>

            </div>

            {/* BUTTONS */}

            <div className="flex gap-5 mt-14">

              <button
                onClick={() => navigate("/home")}
                className="flex-1 border border-gray-300 py-4 rounded-2xl font-medium text-lg hover:bg-gray-100 transition"
              >

                Continue Shopping

              </button>

              <button
                onClick={() => navigate("/profile")}
                className="flex-1 bg-[#284b63] text-white py-4 rounded-2xl font-medium text-lg hover:opacity-90 transition"
              >

                View My Orders

              </button>

            </div>

            {/* SUPPORT */}

            <div className="text-center mt-10 text-gray-500">

              Need help?

              <span className="text-[#284b63] hover:underline cursor-pointer ml-1">

                Contact our support

              </span>

            </div>

          </div>

        </div>

      </div>
</div>
    </StoreLayout>
  );
}