﻿using UnityEngine;
using System.Collections;

public class GreenMagic : ManaItem {

	protected NormalTile tileToDestroy;
	protected ColorBombTile glacierTile;


	public override string ItemName {
		get {
			return "Green Magic";
		}
	}
	
	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);


		TileSwitchInput.Instance.DeactivateDrag();
		Match3Tile.OnTileTap += OnTileSelected;
	}
	
	public override void CancelUsingItem()
	{
		Match3Tile.OnTileTap -= OnTileSelected;
		TileSwitchInput.Instance.ActivateDrag();
		
		base.CancelUsingItem();
	}
	
	void OnTileSelected(AbstractTile tile)
	{
		if (BoardShuffleController.Instance.IsBoardReshuffling) 
		{
			return;
		}
		
		if (!tile.IsMoving && tile.IsDestructible && !tile.IsDestroying && tile.IsUserMoveable && !(tile as NormalTile).IsFrozen()) 
		{
			Match3Tile.OnTileTap -= OnTileSelected;
			tileToDestroy = tile as NormalTile;
			
			effectPosition = tile.cachedTransform;

			MaleficentMatch3Animations.PerformTileUserSelectionAnimation(tile);

			AvatarParticles.Instance.ActivateEffect(AvatarParticlesEffectType.GreenMagic);
			StartItemEffects();
		}
	}
	
	protected override void DoItem()
	{
		if (tileToDestroy) 
		{
			SoundManager.Instance.Play("powerup_greenmagic_sfx");

			AbstractBoardPiece BoardPiece = tileToDestroy.BoardPiece;
			
			BoardPiece.Tile = (boardLogic.boardRenderer as Match3BoardRenderer).SpawnSpecificTileAt(BoardPiece.BoardPosition.row,
			                                                                                        BoardPiece.BoardPosition.col, typeof(ColorBombTile), TileColorType.None);
			glacierTile = BoardPiece.Tile as ColorBombTile;
			
			tileToDestroy.tileModelRenderer.enabled = false;
		}
		
		WaitAndDestroyTile();
	}
	
	void WaitAndDestroyTile()
	{
		
		if (glacierTile) {
			glacierTile.lastNeighborTile = tileToDestroy as NormalTile;
			glacierTile.destroyColor = tileToDestroy.TileColor;
			glacierTile.wasFirstTapped = true;
			glacierTile.Destroy();

			tileToDestroy.useDestroyEffect = false;
			tileToDestroy.Destroy();
		}
		
		TileSwitchInput.Instance.ActivateDrag();
		
		base.DoItem();
		DoDestroy();
	}
	
	protected override void OnDestroy ()
	{
		base.OnDestroy ();
		
		Match3Tile.OnTileTap -= OnTileSelected;
	}
}