import { AppBar, Toolbar, Typography, InputBase, IconButton } from "@mui/material";
import { Search, ShoppingCart, AccountCircle } from "@mui/icons-material";

export default function NavbarHome() {
  return (
    <AppBar position="sticky" sx={{ background: "#0f172a" }}>
      <Toolbar sx={{ justifyContent: "space-between" }}>

        
        <Typography variant="h6" sx={{ fontWeight: 700 }}>
          Seikatsu
        </Typography>

       
        <InputBase
          placeholder="Search products..."
          sx={{
            background: "white",
            padding: "6px 12px",
            borderRadius: "6px",
            width: "40%"
          }}
        />

        {/* Icons */}
        <div>

          <IconButton sx={{ color: "white" }}>
            <Search />
          </IconButton>

          <IconButton sx={{ color: "white" }}>
            <ShoppingCart />
          </IconButton>

          <IconButton sx={{ color: "white" }}>
            <AccountCircle />
          </IconButton>

        </div>

      </Toolbar>
    </AppBar>
  );
}