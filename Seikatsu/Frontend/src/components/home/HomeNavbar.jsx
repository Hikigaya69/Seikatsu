import { ShoppingCart, User,LogOut } from "lucide-react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";

export default function HomeNavbar() {
  const navigate = useNavigate();
const [setCartCount] = useState(0);
useEffect(() => {

  const fetchCartSummary = async () => {

    try {

      const res = await axios.get(
        "/api/Cart/cartsummary",
        { withCredentials: true }
      );

      setCartCount(res.data.data.totalItems);

    } catch (err) {

      console.error(err);

    }

  };

  fetchCartSummary();

}, []);
const handleLogout = async () => {

    try {
        await axios.post(
            "/api/Auth/logout", 
            {},
            { withCredentials: true }
        );
    navigate("/login");

  } catch (err) {

    console.error(err);

  }

};
  return (
    <div className="bg-white border-b p-2 shadow-sm">

      <div className="max-w-7xl mx-auto flex items-center justify-between p-5">

        
        <h1 className="text-2xl font-bold text-[#284b63]">
          Seikatsu
        </h1>

        
        <input
          type="text"
          placeholder="Search product"
          className="w-125 px-4 py-2 border rounded-lg"
        />

        
        <div className="flex gap-6">

          <User className="cursor-pointer" />

          <ShoppingCart
  className="cursor-pointer"
  onClick={() => navigate("/cart")}
/>
          <button
  onClick={handleLogout}
  className="flex items-center gap-2 border border-gray-300 px-3 py-1 rounded-lg shadow-sm hover:bg-gray-100 transition"
>
  <LogOut size={18} />
  Logout
</button>

        </div>

      </div>

    </div>
  );
}