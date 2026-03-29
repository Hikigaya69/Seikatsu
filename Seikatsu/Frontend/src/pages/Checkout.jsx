import { useEffect, useState } from "react";
import api from "../Utils/api"; // ← replace axios import
import StoreLayout from "../layouts/StoreLayout";
import { useNavigate } from "react-router-dom";
import { ArrowLeft } from "lucide-react";

export default function Checkout() {

    const [cart, setCart] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        fetchCart();
    }, []);

    const fetchCart = async () => {
        try {
            const res = await api.post("/Cart/getcart", {});
            setCart(res.data.data);
        } catch (err) {
            console.error(err);
        }
    };

// ... rest of JSX stays exactly the same

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
        <div className="p-10">Loading checkout...</div>
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
      <div className="max-w-7xl mx-auto px-6 py-10">

        
        <div className="bg-[#3c6e71] text-white rounded-lg py-3 text-center font-semibold text-xl mb-8">
          Checkout ({cart.items.length} items)
        </div>

        <div className="grid grid-cols-3 gap-8">

          {/* LEFT SIDE */}
          <div className="col-span-2 space-y-6">

            {/* ADDRESS */}
            <div className="bg-white rounded-xl shadow p-6">

              <div className="flex justify-between">

                <h2 className="font-semibold text-lg">
                  Delivering to address
                </h2>

                <button className="text-[#284b63]">
                  Change
                </button>

              </div>

              <p className="text-gray-600 mt-4">
                Add your delivery address here
              </p>

            </div>


            {/* PAYMENT METHOD */}
            <div className="bg-white rounded-xl shadow p-6">

              <h2 className="font-semibold text-lg mb-4">
                Payment Method
              </h2>

              <div className="space-y-4">

                <div className="border rounded-lg p-4">
                  Credit / Debit Card / NetBanking
                  <p className="text-sm text-gray-500">
                    Visa • MasterCard • RuPay
                  </p>
                </div>

                <div className="border rounded-lg p-4">
                  UPI

                  <input
                    type="text"
                    placeholder="Enter UPI ID"
                    className="mt-2 w-full border rounded px-3 py-2"
                  />

                </div>

              </div>

            </div>

          </div>


          {/* RIght side SUMMARY */}
          <div className="bg-white rounded-xl shadow p-6 h-fit">

            <h2 className="font-semibold text-lg mb-4">
              Checkout Summary
            </h2>


            {/* ITEM LIST */}
            <div className="space-y-3 mb-4">

              {cart.items.map((item) => (

                <div
                  key={item.cartItemId}
                  className="flex justify-between text-sm"
                >

                  <span>
                    {item.productName} ×{item.quantity}
                  </span>

                  <span>
                    ¥{item.itemTotal}
                  </span>

                </div>

              ))}

            </div>


            <hr className="my-4" />


            {/* TOTAL */}
            <div className="flex justify-between font-bold text-lg mb-6">

              <span>Total</span>

              <span>
                ¥{cart.totalPrice}
              </span>

            </div>


            <button className="w-full bg-[#284b63] text-white py-3 rounded-lg">
              Proceed to Payment
            </button>

          </div>

        </div>

      </div>

    </StoreLayout>

  );

}